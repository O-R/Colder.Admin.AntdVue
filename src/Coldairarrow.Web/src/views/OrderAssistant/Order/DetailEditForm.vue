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
        <a-form-model-item label="原始订单编号" prop="OrderNo">
          {{ entity.OrderNo }}
        </a-form-model-item>
        <a-form-model-item label="客户账号" prop="CustomerNo">
          {{ entity.CustomerNo }}
        </a-form-model-item>
        <a-form-model-item label="客户名" prop="CustomerName">
          {{ entity.CustomerName }}
        </a-form-model-item>
        <a-form-model-item label="省" prop="Province">
          <a-input v-model="entity.Province" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="市" prop="City">
          <a-input v-model="entity.City" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="区" prop="Area">
          <a-input v-model="entity.Area" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="收货地址" prop="Address">
          <a-textarea v-model="entity.Address" placeholder="" :rows="4" autocomplete="off"/>
        </a-form-model-item>
        <a-form-model-item label="收货人姓名" prop="Receiver">
          <a-input v-model="entity.Receiver" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="收货人手机" prop="ReceiverPhone">
          <a-input v-model="entity.ReceiverPhone" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="商品名称" prop="SkuName">
          <a-input v-model="entity.SkuName" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="商品编号/SKU编号" prop="SkuNo">
          <a-select
            :default-value="entity.SkuNo"
            show-search
            v-model="entity.SkuNo"
            placeholder="请选择SKU编号"
            option-filter-prop="children"
            :filter-option="filterOption"
            @change="handleSkuChange"
          >
            <a-select-option
              v-for="(item, index) in skuData"
              :key="index"
              :value="item.Id">
              {{ item.SkuNo }}
            </a-select-option>
          </a-select>
        </a-form-model-item>
        <a-form-model-item label="数量" prop="Count">
          <a-input-number
            v-model="entity.Count"
            :min="1"
            :precision="0"
            style="margin: -5px 0;width: 100%"
            autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="单价" prop="Price">
          <a-input-number
            v-model="entity.Price"
            :min="0"
            :precision="2"
            style="margin: -5px 0;width: 100%"
            autocomplete="off" />
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
      skuData: [],
      rules: {
        Province: [{ required: true, message: '必填', trigger: 'blur' }],
        City: [{ required: true, message: '必填', trigger: 'blur' }],
        Area: [{ required: true, message: '必填', trigger: 'blur' }],
        Address: [{ required: true, message: '必填', trigger: 'blur' }],
        Receiver: [{ required: true, message: '必填', trigger: 'blur' }],
        ReceiverPhone: [{ required: true, message: '必填', trigger: 'blur' }],
        SkuName: [{ required: true, message: '必填', trigger: 'blur' }],
        SkuNo: [{ required: true, message: '必填', trigger: 'change' }],
        Count: [{ required: true, message: '必填', trigger: 'blur' }],
        Price: [{ required: true, message: '必填', trigger: 'blur' }]
      },
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
      this.title = title
      if (id) {
        this.loading = true
        this.$http.post('/OrderAssistant/Order/GetTheItem', { id: id }).then(resJson => {
          this.loading = false
          this.entity = resJson.Data
          this.getSkuDataList()
        })
      }
    },
    handleSubmit () {
      this.$refs['form'].validate(valid => {
        if (!valid) {
          return
        }
        this.loading = true
        if (this.entity.Count === 0) {
          this.entity.Count = 1
        }

        this.$http.post('/OrderAssistant/Order/SaveItem', this.entity).then(resJson => {
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
    },
    getSkuDataList () {
      var customerId = this.entity.CustomerId
      this.loading = true
      this.$http
        .post('/OrderAssistant/Customer/GetSkuList', {
          CustomerId: customerId
        })
        .then(resJson => {
          this.skuData = resJson.Data
          this.loading = false
        })
    },
    filterOption (input, option) {
      return (
        option.componentOptions.children[0].text.toLowerCase().indexOf(input.toLowerCase()) >= 0
      )
    },
    handleSkuChange (value) {
      const sku = this.skuData.filter(sd => sd.Id === value)[0]
      this.entity.SkuId = value
      this.entity.SkuNo = sku.SkuNo
      this.entity.SkuName = sku.SkuName
      this.entity.Price = sku.Price
    }
  }
}
</script>
