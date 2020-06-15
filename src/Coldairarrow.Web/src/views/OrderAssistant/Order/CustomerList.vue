<template>
  <a-form :form="form">
    <a-form-item label="下单客户：" :labelCol="labelCol" :wrapperCol="wrapperCol">
      <a-select
        show-search
        placeholder="请选择客户"
        option-filter-prop="children"
        :filter-option="filterOption"
        :loading="loading"
        @focus="handleFocus"
        @blur="handleBlur"
        @change="handleChange"
      >
        <a-select-option v-for="(item, index) in data" :key="index" :value="item.Id">
          {{ item.CustomerName }}
        </a-select-option>
      </a-select>
    </a-form-item>
  </a-form>
</template>
<script>
export default {
  mounted () {
    this.getDataList()
  },

  // :defaultValue="selectedCustomerName"
  // computed: {
  //   selectedCustomerName () {
  //     return this.$store.state.app.selectedCustomerId
  //   }
  // },
  data () {
    return {
      data: [],
      customerId: '',
      pagination: {
        current: 1,
        pageSize: 10
      },
      filters: {},
      sorter: { field: 'Id', order: 'asc' },
      loading: false,
      queryParam: {},
      form: this.$form.createForm(this),
      labelCol: { xs: { span: 24 }, sm: { span: 7 } },
      wrapperCol: { xs: { span: 24 }, sm: { span: 13 } }
    }
  },
  methods: {
    getDataList () {
      this.selectedRowKeys = []

      this.loading = true
      this.$http
        .post('/OrderAssistant/Customer/GetDataList', {
          PageIndex: this.pagination.current,
          SortField: this.sorter.field || 'Id',
          SortType: this.sorter.order,
          Search: this.queryParam,
          ...this.filters
        })
        .then(resJson => {
          this.loading = false
          this.data = resJson.Data
          const pagination = { ...this.pagination }
          pagination.total = resJson.Total
          this.pagination = pagination
        })
    },
    // getCustomerId () {
    //   return this.customerId
    // },
    handleChange (value) {
      this.$store.commit('SET_SELECTED_CUSTOMERID', value)
      // this.customerId = value
      // this.$emit('changeCustomerId', this.customerId)
    },
    handleBlur () {
    //   console.log('blur');
    },
    handleFocus () {
    //   console.log('focus');
    },
    filterOption (input, option) {
      return (
        option.componentOptions.children[0].text.toLowerCase().indexOf(input.toLowerCase()) >= 0
      )
    }
  }
}
</script>
