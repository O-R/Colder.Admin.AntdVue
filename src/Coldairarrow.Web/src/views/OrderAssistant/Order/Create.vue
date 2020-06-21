<template>

  <a-card :bordered="false">
    <div>
      <a-steps :current="current">
        <a-step v-for="item in steps" :key="item.title" :title="item.title" />
      </a-steps>
      <div class="steps-content">
        <customer-list ref="customerList" v-show="current==0"></customer-list>
        <import-list ref="importList" v-show="current==1" :loading="isImportListLoading"></import-list>
        <parse-list ref="parseList" v-show="current==2" :loading="isSaveLoading"></parse-list>
      </div>
      <div class="steps-action">
        <a-button v-if="current > 0" style="margin-right: 20px" @click="prev" :disabled="isNextLoading || isSaveLoading">
          上一步
        </a-button>
        <a-button v-if="current < steps.length - 1" type="primary" @click="next" :loading="isNextLoading">
          下一步
        </a-button>
        <a-button
          v-if="current == steps.length - 1"
          type="primary"
          @click="save"
          :loading="isSaveLoading"
        >
          保存&关闭
        </a-button>
      </div>
    </div>
    <a-modal v-model="visible" title="拆词结果" on-ok="handleSaveValidate">
      <template slot="footer">
        <a-button key="beforeSave" type="primary" @click="handleSaveValidate">
          我知道了
        </a-button>
      </template>
      <p>存在 {{ ErrorRowCount }} 条异常数据，请先修改后再保存</p>
    </a-modal>
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
      isNextClick: false,
      isSaveClick: false,
      current: 0,
      ErrorRowCount: 0,
      visible: false,
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
  computed: {
    isNextLoading: function () {
      return this.loading && this.isNextClick
    },
    isSaveLoading: function () {
      return this.loading && this.isSaveClick
    },
    isImportListLoading: function () {
      return this.loading && this.isNextClick && this.current === 1
    }
  },
  beforeRouteEnter (to, from, next) {
    next(vm => {
    // 通过 `vm` 访问组件实例
      vm.$refs.customerList.getDataList()
    })
  },
  methods: {
    next () {
      this.isNextClick = true
      var customerId = this.$store.state.app.selectedCustomerId
      if (this.current === 0) {
        // var customerId = this.$refs.customerList.getCustomerId()
        if (customerId === '' || customerId === undefined || customerId == null) {
          this.$message.warn('请先选择客户!')
        } else {
          this.current++
        }
        this.isNextClick = false
      } else if (this.current === 1) {
        this.loading = true

        var parseData = []
        try {
          parseData = this.$refs.importList.getParseData()
        } catch (error) {
          this.$message.error('解析数据异常，请检查录入信息!')
          this.loading = false
          this.isNextClick = false
          return
        }
        if (parseData.length === 0) {
          this.$message.warn('请先录入信息!')
          this.loading = false
          this.isNextClick = false
        } else {
          const postData = {
            // CustomerId: this.$refs.customerList.getCustomerId(),
            CustomerId: customerId,
            Orders: parseData
          }

          this.$http.post('/OrderAssistant/Order/Parse', postData).then(resJson => {
            this.loading = false
            this.isNextClick = false

            if (resJson.Success) {
              if (resJson.Data === undefined || resJson.Data == null || resJson.Data.length === 0) {
                this.$message.error('解析失败，找不到相关商品信息')
              } else {
                this.current++
                this.$nextTick(() => {
                  this.$refs.parseList.setData(resJson.Data)
                })
              }
            } else {
              this.$message.error(resJson.Msg)
            }
          })
        }
      }
    },
    prev () {
      if (this.current === 2) {
        var that = this
        this.$confirm({
          title: '返回上一步将丢失当前步骤所有数据，是否确定?',
          onOk () {
            that.$refs.parseList.cancelEdit()
            that.current--
          }
        })
      } else {
        this.current--
      }
    },
    handleSaveValidate () {
      this.visible = false
      this.ErrorRowCount = 0
    },
    save () {
      var orderParseList = this.$refs.parseList.getData()

      if (orderParseList.length <= 0) {
        this.$message.warn('没有可保存的数据')
        return
      }

      this.ErrorRowCount = orderParseList.reduce(function (sum, item) {
        return sum + (item.IsError ? 1 : 0)
      }, 0)

      if (this.ErrorRowCount > 0) {
        this.visible = true
        return
      }

      this.isSaveClick = true
      this.loading = true
      this.$http.post('/OrderAssistant/Order/SaveList', orderParseList).then(resJson => {
        if (resJson.Success) {
          var that = this
          this.$message.success('保存成功', 1, function () {
            that.loading = false
            that.isSaveClick = false
            that.current = 0
          })
        } else {
          this.loading = false
          this.isSaveClick = false
          this.$message.error(resJson.Msg)
        }
      })
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
