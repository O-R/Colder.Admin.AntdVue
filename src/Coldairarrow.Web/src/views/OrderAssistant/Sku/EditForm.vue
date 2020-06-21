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

        <a-card title="基础信息" :bordered="false" class="card-tab">
          <a-form-model-item label="SKU编号" prop="SkuNo">
            <a-input v-model="entity.SkuNo" autocomplete="off" />
          </a-form-model-item>
          <a-form-model-item label="SKU名称" prop="SkuName">
            <a-input v-model="entity.SkuName" autocomplete="off" />
          </a-form-model-item>

          <a-form-model-item
            v-for="(keyword, index) in entity.SkuKeywords"
            :key="keyword.key"
            v-bind="index === 0 ? layout : formItemLayoutWithOutLabel"
            :label="index === 0 ? '关键词' : ''"
            :prop="'SkuKeywords.' + index + '.word'"
            :rules="{
              required: true,
              message: '关键词必填',
              trigger: 'blur',
            }"
          >
            <a-input
              v-model="keyword.word"
              style="width: 70%;margin-right: 5px"
              placeholder="关键词"
            />
            <a-icon
              v-if="entity.SkuKeywords.length >= 0"
              class="dynamic-delete-button"
              type="minus-circle-o"
              :disabled="entity.SkuKeywords.length === 0"
              @click="removeKeyword(keyword)"
            />
          </a-form-model-item>
          <a-form-model-item v-bind="formItemLayoutWithOutLabel">
            <a-button type="dashed" style="width: 40%" @click="addKeyword">
              <a-icon type="plus" /> 添加
            </a-button>
          </a-form-model-item>

        </a-card>

        <a-card title="定价信息" :bordered="false" class="card-tab" >
          <a-form-model-item
            v-for="(domain, index) in entity.SkuCustomers"
            :key="domain.key"
            v-bind="index === 0 ? layout : formItemLayoutWithOutLabel"
            :label="index === 0 ? '客户名' : ''"
            :prop="'SkuCustomers.' + index + '.CustomerId'"
            :rules="{
              validator: validateCustomerPrice,
              trigger: ['change','blur'],
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
            <a-input-number
              v-model="domain.Price"
              :min="0"
              :precision="2"
              style="width: 20%; margin-left: 10px;margin-right: 5px"
              placeholder="单价"
            />
            元
            <a-icon
              v-if="entity.SkuCustomers.length >= 0"
              class="dynamic-delete-button"
              type="minus-circle-o"
              :disabled="entity.SkuCustomers.length === 0"
              @click="removeDomain(domain)"
            />
          </a-form-model-item>
          <a-form-model-item v-bind="formItemLayoutWithOutLabel">
            <a-button type="dashed" style="width: 40%" @click="addDomain">
              <a-icon type="plus" /> 添加
            </a-button>
          </a-form-model-item>

        </a-card>
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
        SkuNo: '',
        SkuName: '',
        SkuCustomers: [{
          key: Date.now()
        }],
        SkuKeywords: [{
          key: Date.now(),
          word: ''
        }]
      },
      rules: {
        SkuNo: [
          { required: true, message: 'SKU编号必填', trigger: 'blur' }
        ],
        SkuName: [{ required: true, message: 'SKU名称必填', trigger: 'blur' }]
      },
      title: ''

    }
  },
  methods: {
    init () {
      this.visible = true
      this.entity = {
        SkuNo: '',
        SkuName: '',
        SkuCustomers: [{
          key: Date.now()
        }],
        SkuKeywords: [{
          key: Date.now(),
          word: ''
        }]
      }
      this.$nextTick(() => {
        this.$refs['form'].clearValidate()
      })
    },
    // data () {

    // },
    openForm (id, title) {
      this.title = title
      this.init()

      if (id) {
        this.loading = true
        this.$http.post('/OrderAssistant/Sku/GetTheData', { id: id }).then(resJson => {
          this.loading = false
          if (resJson.Data.KeywordList.length > 0) {
            resJson.Data.SkuKeywords = resJson.Data.KeywordList.map((k, idx) => { return { key: Date.now() + '' + idx, word: k } })
          }
          if (resJson.Data.SkuCustomers.length > 0) {
            resJson.Data.SkuCustomers.forEach(function (k, index, arr) {
              k.key = k.Id
            })
          }
          // this.entity = resJson.Data
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
        this.entity.KeyWords = this.entity.SkuKeywords.map(k => k.word).toString()
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
    removeKeyword (item) {
      const index = this.entity.SkuKeywords.indexOf(item)
      if (index !== -1) {
        this.entity.SkuKeywords.splice(index, 1)
      }
    },
    addKeyword () {
      this.entity.SkuKeywords.push({
        key: Date.now(),
        word: ''
      })
    },
    removeDomain (item) {
      const index = this.entity.SkuCustomers.indexOf(item)
      if (index !== -1) {
        this.entity.SkuCustomers.splice(index, 1)
      }
    },
    addDomain () {
      this.entity.SkuCustomers.push({
        key: Date.now()
      })
    },
    filterOption (input, option) {
      return (
        option.componentOptions.children[0].text.toLowerCase().indexOf(input.toLowerCase()) >= 0
      )
    },
    validateCustomerPrice (rule, value, callback) {
      if (value === '') {
        callback(new Error('请选择客户'))
      } else {
        var priceInfo = this.entity.SkuCustomers.find(sc => sc.CustomerId === value)
        if (priceInfo === undefined) {
          callback(new Error('客户被删除，请重新选择'))
        } else if (priceInfo.Price === '') {
          callback(new Error('单价必填'))
        } else {
          if (!/^\d+(\.\d+)?$/.test(priceInfo.Price)) {
            callback(new Error('单价必须是数字'))
          }
        }
      }
      callback()
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
.card-tab .ant-card-body {
    padding: 24px 0px 0px 0px;
}
</style>
