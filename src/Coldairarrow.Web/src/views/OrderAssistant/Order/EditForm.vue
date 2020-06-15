<template>
  <a-modal
    :title="title"
    width="40%"
    :visible="visible"
    :confirmLoading="loading"
    @ok="handleSubmit"
    @cancel="()=>{this.visible=false}"
  >
    <a-spin :spinning="loading">
      <a-form-model ref="form" :model="entity" :rules="rules" v-bind="layout">
        <a-form-model-item label="订单编号" prop="OrderNo">
          <a-input v-model="entity.OrderNo" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="客户编号" prop="CustomerNo">
          <a-input v-model="entity.CustomerNo" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="客户名" prop="CustomerName">
          <a-input v-model="entity.CustomerName" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="客户Id" prop="CustomerId">
          <a-input v-model="entity.CustomerId" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="ProvinceNo" prop="ProvinceNo">
          <a-input v-model="entity.ProvinceNo" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="省" prop="Province">
          <a-input v-model="entity.Province" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="CityNo" prop="CityNo">
          <a-input v-model="entity.CityNo" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="市" prop="City">
          <a-input v-model="entity.City" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="AreaNo" prop="AreaNo">
          <a-input v-model="entity.AreaNo" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="区" prop="Area">
          <a-input v-model="entity.Area" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="详细地址" prop="Address">
          <a-input v-model="entity.Address" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="收货人" prop="Receiver">
          <a-input v-model="entity.Receiver" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="收货人手机号" prop="ReceiverPhone">
          <a-input v-model="entity.ReceiverPhone" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="折扣" prop="Discount">
          <a-input v-model="entity.Discount" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="总价" prop="TotalPrice">
          <a-input v-model="entity.TotalPrice" autocomplete="off" />
        </a-form-model-item>
      </a-form-model>
    </a-spin>
  </a-modal>
</template>

<script>
export default {
  props: {
    parentObj: Object
  },
  data () {
    return {
      layout: {
        labelCol: { span: 5 },
        wrapperCol: { span: 18 }
      },
      visible: false,
      loading: false,
      entity: {},
      rules: {},
      title: ''
    }
  },
  methods: {
    init () {
      this.visible = true
      this.entity = {}
      this.$nextTick(() => {
        this.$refs['form'].clearValidate()
      })
    },
    openForm (id, title) {
      this.init()

      if (id) {
        this.loading = true
        this.$http.post('/OrderAssistant/Order/GetTheData', { id: id }).then(resJson => {
          this.loading = false

          this.entity = resJson.Data
        })
      }
    },
    handleSubmit () {
      this.$refs['form'].validate(valid => {
        if (!valid) {
          return
        }
        this.loading = true
        this.$http.post('/OrderAssistant/Order/SaveData', this.entity).then(resJson => {
          this.loading = false

          if (resJson.Success) {
            this.$message.success('操作成功!')
            this.visible = false

            this.parentObj.getDataList()
          } else {
            this.$message.error(resJson.Msg)
          }
        })
      })
    }
  }
}
</script>
