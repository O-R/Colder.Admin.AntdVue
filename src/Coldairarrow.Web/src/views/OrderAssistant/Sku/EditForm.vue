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
      <a-form-model
        :model="entity"
        :rules="rules"
        ref="form"
        v-bind="layout">
        <a-form-model-item label="sku编号" prop="SkuNo">
          <a-input v-model="entity.SkuNo" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="sku名称" prop="SkuName">
          <a-input v-model="entity.SkuName" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="关键词" prop="KeyWords">
          <a-textarea v-model="entity.KeyWords" placeholder="" :rows="4" autocomplete="off"/>
        </a-form-model-item>

        <a-form-model-item
          v-for="(domain, index) in entity.domains"
          :key="domain.key"
          v-bind="index === 0 ? layout : formItemLayoutWithOutLabel"
          :label="index === 0 ? '客户名' : ''"
          :prop="'domains.' + index + '.CustomerId'"
          :rules="{
            required: true,
            message: 'domain can not be null',
            trigger: 'blur',
          }"
        >
          <a-select
            show-search
            placeholder="请选择客户"
            v-model="domain.CustomerId"
            option-filter-prop="children"
            :filter-option="filterOption"
            style="width: 50%; margin-right: 10px"
          >
            <a-select-option
              v-for="(item, idx) in customerData"
              :key="idx"
              :value="item.Id">
              {{ item.CustomerName }}
            </a-select-option>
          </a-select>
          --
          <a-input
            v-model="domain.Price"
            style="width: 30%; margin-left: 10px;margin-right: 5px"
            placeholder="单价"
          />
          元
          <a-icon
            v-if="entity.domains.length > 1"
            class="dynamic-delete-button"
            type="minus-circle-o"
            :disabled="entity.domains.length === 1"
            @click="removeDomain(domain)"
          />
        </a-form-model-item>
        <a-form-model-item v-bind="formItemLayoutWithOutLabel">
          <a-button type="dashed" style="width: 40%" @click="addDomain">
            <a-icon type="plus" /> 添加
          </a-button>
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
  computed: {
    customerData () {
      return { ...this.parentObj.customerData }
    }
  },
  data () {
    return {
      layout: {
        labelCol: { span: 5 },
        wrapperCol: { span: 18 }
      },
      formItemLayoutWithOutLabel: {
        wrapperCol: {
          span: 18, offset: 5
        }
      },
      visible: false,
      loading: false,
      entity: {
        domains: [{
          CustomerId: '',
          key: Date.now(),
          Price: 0
        }]
      },
      rules: {},
      title: ''

    }
  },
  methods: {
    init () {
      this.visible = true
      // this.entity = {}
      this.$nextTick(() => {
        this.$refs['form'].clearValidate()
      })
    },
    openForm (id, title) {
      this.title = title
      this.init()

      if (id) {
        this.loading = true
        this.$http.post('/OrderAssistant/Sku/GetTheData', { id: id }).then(resJson => {
          this.loading = false
          Object.assign(this.entity, resJson.Data)
        })
      }
    },
    handleSubmit () {
      this.$refs['form'].validate(valid => {
        if (!valid) {
          return
        }
        this.loading = true
        this.$http.post('/OrderAssistant/Sku/SaveData', this.entity).then(resJson => {
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
    removeDomain (item) {
      const index = this.entity.domains.indexOf(item)
      if (index !== -1) {
        this.entity.domains.splice(index, 1)
      }
    },
    addDomain () {
      this.entity.domains.push({
        CustomerId: '',
        key: Date.now()
      })
    },
    filterOption (input, option) {
      return (
        option.componentOptions.children[0].text.toLowerCase().indexOf(input.toLowerCase()) >= 0
      )
    }
  }
}
</script>
<style>
.dynamic-delete-button {
  cursor: pointer;
  position: relative;
  top: 4px;
  font-size: 24px;
  color: #999;
  transition: all 0.3s;
}
.dynamic-delete-button:hover {
  color: #777;
}
.dynamic-delete-button[disabled] {
  cursor: not-allowed;
  opacity: 0.5;
}
</style>
