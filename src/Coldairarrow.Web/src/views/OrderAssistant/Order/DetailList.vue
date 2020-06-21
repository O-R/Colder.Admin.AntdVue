<template>
  <a-card :bordered="false">

    <div class="table-operator">
      <a-button type="primary" :loading="loading" @click="exportExcel" >
        导出excel
      </a-button>
    </div>

    <div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="10">
          <a-col :md="4" :sm="24">
            <a-form-item label="查询类别">
              <a-select allowClear v-model="queryParam.condition">
                <a-select-option key="Order.OrderNo">订单编号</a-select-option>
                <a-select-option key="Order.CustomerNo">客户账号</a-select-option>
                <a-select-option key="SkuNo">SKU编号</a-select-option>
                <a-select-option key="Order.ReceiverPhone">收货人手机号</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :md="4" :sm="24">
            <a-form-item>
              <a-input v-model="queryParam.keyword" placeholder="关键字" />
            </a-form-item>
          </a-col>
          <a-col :md="6" :sm="24">
            <a-button type="primary" @click="getDataList">查询</a-button>
            <a-button style="margin-left: 8px" @click="() => (queryParam = {})">重置</a-button>
          </a-col>
        </a-row>
      </a-form>
    </div>

    <a-table
      :columns="columns"
      :data-source="data"
      :scroll="{ x: '200%' }"
      :loading="loading"
      :rowKey="row => row.Id"
      :pagination="pagination"
      @change="handleTableChange"
      :rowSelection="{ selectedRowKeys: selectedRowKeys, onChange: onSelectChange }"
      bordered >
      <span slot="operation" slot-scope="text, record">
        <template>
          <a @click="handleEdit(record.Id)">编辑</a>
        </template>
      </span>
    </a-table>

    <detail-edit-form ref="editForm" :parentObj="this"></detail-edit-form>
  </a-card>
</template>
<script>
import Excel from '@/utils/excel.js'
import moment from 'moment'
import DetailEditForm from './DetailEditForm'

const columns = [
  {
    title: '原始订单编号',
    dataIndex: 'OrderNo',
    width: '10%',
    // fixed: 'left',
    scopedSlots: { customRender: 'OrderNo' }
  },
  {
    title: '生成时间',
    width: '10%',
    // fixed: 'left',
    dataIndex: 'CreateTime',
    scopedSlots: { customRender: 'CreateTime' }
  },
  {
    title: '客户账号',
    width: '10%',
    // fixed: 'left',
    dataIndex: 'CustomerNo',
    scopedSlots: { customRender: 'CustomerNo' }
  },
  {
    title: '省',
    dataIndex: 'Province',
    width: '5%',
    scopedSlots: { customRender: 'Province' }
  },
  {
    title: '市',
    width: '5%',
    dataIndex: 'City',
    scopedSlots: { customRender: 'City' }
  },
  {
    title: '区',
    width: '5%',
    dataIndex: 'Area',
    scopedSlots: { customRender: 'Area' }
  },
  {
    title: '收货地址',
    width: '20%',
    dataIndex: 'Address',
    scopedSlots: { customRender: 'Address' }
  },
  {
    title: '收货人姓名',
    width: '10%',
    dataIndex: 'Receiver',
    scopedSlots: { customRender: 'Receiver' }
  },
  {
    title: '收货人手机',
    width: '10%',
    dataIndex: 'ReceiverPhone',
    scopedSlots: { customRender: 'ReceiverPhone' }
  },
  {
    title: '商品名称',
    width: '20%',
    dataIndex: 'SkuName',
    scopedSlots: { customRender: 'SkuName' }
  },
  {
    title: '商品编号/SKU编号',
    width: '20%',
    dataIndex: 'SkuNo',
    scopedSlots: { customRender: 'SkuNo' }
  },
  {
    title: '数量',
    width: '5%',
    dataIndex: 'Count',
    scopedSlots: { customRender: 'Count' }
  },
  {
    title: '单价',
    width: '5%',
    dataIndex: 'Price',
    scopedSlots: { customRender: 'Price' }
  },
  {
    title: '操作',
    dataIndex: 'operation',
    // fixed: 'right',
    width: '5%',
    scopedSlots: { customRender: 'operation' }
  }
]

export default {
  components: {
    DetailEditForm
  },
  mounted () {
    this.getDataList()
  },
  data () {
    return {
      data: [],
      columns,
      filters: {},
      sorter: { field: 'Id', order: 'desc' },
      loading: false,
      queryParam: {},
      selectedRowKeys: [],
      pagination: {
        current: 1,
        pageSize: 20,
        showTotal: (total, range) => `总数:${total} 当前:${range[0]}-${range[1]}`,
        showSizeChanger: true,
        pageSizeOptions: ['20', '40', '60', '80', '100']
      }
    }
  },
  methods: {
    handleTableChange (pagination, filters, sorter) {
      this.pagination = { ...pagination }
      this.filters = { ...filters }
      this.sorter = { ...sorter }
      this.getDataList()
    },
    getDataList () {
      this.selectedRowKeys = []

      this.loading = true
      this.$http
        .post('/OrderAssistant/Order/GetItemList', {
          PageIndex: this.pagination.current,
          PageRows: this.pagination.pageSize,
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
    onSelectChange (selectedRowKeys) {
      this.selectedRowKeys = selectedRowKeys
    },
    hasSelected () {
      return this.selectedRowKeys.length > 0
    },
    handleEdit (id) {
      this.$refs.editForm.openForm(id, '编辑订单')
    },
    exportExcel () {
      if (this.data.length <= 0) {
        this.$message.warn('请先录入订单信息')
        return
      }
      const excelFields = {
        '原始订单编号': 'OrderNo',
        '生成时间': 'CreateTime',
        '买家账号': 'CustomerNo',
        '国家': '',
        '省': 'Province',
        '市': 'City',
        '区': 'Area',
        '收货地址': 'Address',
        '买家姓名': 'Receiver',
        '买家电话': '',
        '买家手机': 'ReceiverPhone',
        '身份证号码': '',
        '线上宝贝名称': '',
        '线上销售属性': '',
        '商品名称': 'SkuName',
        '商品编号/SKU编号': 'SkuNo',
        '数量': 'Count',
        '单价': 'Price',
        '交易拍下时间': '',
        '交易付款时间': '',
        '物流公司': '',
        '物流单号': '',
        '税金': '',
        '商城扣费': '',
        '支付交易号': '',
        '邮编': '',
        '买家运费': '',
        '发票抬头': '',
        '买家留言': '',
        '卖家备注': '',
        '货到付款': '',
        '明细-服务费': '',
        '门店名称': '',
        '税率(%)': '',
        '明细-商城扣费': ''
      }
      var t = moment().format('YYYYMMDDHHmmssSSS')
      var fileId = '订单-' + t
      Excel.exportExcel(this.data, excelFields, fileId)
    }
  }
}
</script>
