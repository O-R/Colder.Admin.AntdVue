<template>

  <a-card :bordered="false">
    <div>
      <a-steps :current="current">
        <a-step v-for="item in steps" :key="item.title" :title="item.title" />
      </a-steps>
      <div class="steps-content">
        <customer-list ref="customerList" v-show="current==0"></customer-list>
        <import-list ref="importList" v-show="current==1" :loading="loading"></import-list>
        <parse-list ref="parseList" v-show="current==2"></parse-list>
      </div>
      <div class="steps-action">
        <a-button v-if="current > 0" style="margin-right: 20px" @click="prev">
          上一步
        </a-button>
        <a-button v-if="current < steps.length - 1" type="primary" @click="next">
          下一步
        </a-button>
        <a-button
          v-if="current == steps.length - 1"
          type="primary"
          @click="$message.success('Processing complete!')"
        >
          保存
        </a-button>
      </div>
    </div>
  </a-card>
</template>
<script>
import CustomerList from './CustomerList'
import ImportList from './ImportList'
import ParseList from './ParseList'

export default {
  components: {
    CustomerList,
    ImportList,
    ParseList
  },
  data () {
    return {
      loading: false,
      current: 0,
      steps: [
        {
          title: '请选择下单客户',
          content: 'First-content'
        },
        {
          title: '请录入信息',
          content: 'Second-content'
        },
        {
          title: '完成',
          content: 'Last-content'
        }
      ]
    }
  },
  methods: {
    next () {
      if (this.current === 0) {
        var customerId = this.$refs.customerList.getCustomerId()
        if (customerId === '') {
          this.$message.warn('请先选择客户!')
          return
        }
        this.current++
        return
      }
      if (this.current === 1) {
        this.loading = true
        var parseData = this.$refs.importList.getParseData()
        if (parseData.length === 0) {
          this.$message.warn('请先录入信息!')
          return
        }

        console.log(parseData)

        const defaultItem = {
          index: 0,
          province: '',
          provinceCode: '',
          city: '',
          cityCode: '',
          county: '',
          countyCode: '',
          street: '',
          streetCode: '',
          address: '',
          name: '',
          phone: '',
          skuKeyWords: '',
          fullAddress: ''
        }
        const list = parseData.map(item => {
          const dItem = { ...defaultItem }
          Object.assign(dItem, item)
          return dItem
        })

        const postData = {
          CustomerId: this.$refs.customerList.getCustomerId(),
          Orders: list
        }

        this.$http.post('/OrderAssistant/Order/Parse', postData).then(resJson => {
          this.loading = false

          if (resJson.Success) {
            this.current++
            this.$nextTick(() => {
              this.$refs.parseList.setData(resJson.Data)
            })
          } else {
            this.$message.error(resJson.Msg)
          }
        })
      }
    },
    prev () {
      this.current--
    }
  }
}
</script>
<style scoped>
.steps-content {
  margin-top: 16px;
  border: 1px dashed #e9e9e9;
  border-radius: 6px;
  background-color: #fafafa;
  min-height: 200px;
  text-align: center;
  padding: 40px 20px 10px 10px;
}

.steps-action {
  margin-top: 24px;
  text-align: center;
}
</style>
