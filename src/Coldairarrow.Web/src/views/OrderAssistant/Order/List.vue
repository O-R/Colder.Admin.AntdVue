<template>
  <a-card :bordered="false">
    <div class="table-operator">
      <a-button type="primary" icon="plus" @click="hanldleAdd()">新建</a-button>
      <a-button
        type="primary"
        icon="minus"
        @click="handleDelete(selectedRowKeys)"
        :disabled="!hasSelected()"
        :loading="loading"
      >删除</a-button>
      <a-button type="primary" icon="redo" @click="getDataList()">刷新</a-button>
    </div>

    <div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="10">
          <a-col :md="4" :sm="24">
            <a-form-item label="查询类别">
              <a-select allowClear v-model="queryParam.condition">
                <a-select-option key="OrderNo">订单编号</a-select-option>
                <a-select-option key="CustomerNo">客户编号</a-select-option>
                <a-select-option key="CustomerName">客户名</a-select-option>
                <a-select-option key="CustomerId">客户Id</a-select-option>
                <a-select-option key="ProvinceNo">ProvinceNo</a-select-option>
                <a-select-option key="Province">省</a-select-option>
                <a-select-option key="CityNo">CityNo</a-select-option>
                <a-select-option key="City">市</a-select-option>
                <a-select-option key="AreaNo">AreaNo</a-select-option>
                <a-select-option key="Area">区</a-select-option>
                <a-select-option key="Address">详细地址</a-select-option>
                <a-select-option key="Receiver">收货人</a-select-option>
                <a-select-option key="ReceiverPhone">收货人手机号</a-select-option>
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
      ref="table"
      :columns="columns"
      :rowKey="row => row.Id"
      :dataSource="data"
      :pagination="pagination"
      :loading="loading"
      @change="handleTableChange"
      :rowSelection="{ selectedRowKeys: selectedRowKeys, onChange: onSelectChange }"
      :bordered="true"
      size="small"
    >
      <span slot="action" slot-scope="text, record">
        <template>
          <a @click="handleEdit(record.Id)">编辑</a>
          <a-divider type="vertical" />
          <a @click="handleDelete([record.Id])">删除</a>
        </template>
      </span>
    </a-table>

    <edit-form ref="editForm" :parentObj="this"></edit-form>
  </a-card>
</template>

<script>
import EditForm from './EditForm'

const columns = [
  { title: '订单编号', dataIndex: 'OrderNo', width: '10%' },
  { title: '客户编号', dataIndex: 'CustomerNo', width: '10%' },
  { title: '客户名', dataIndex: 'CustomerName', width: '10%' },
  { title: '客户Id', dataIndex: 'CustomerId', width: '10%' },
  { title: 'ProvinceNo', dataIndex: 'ProvinceNo', width: '10%' },
  { title: '省', dataIndex: 'Province', width: '10%' },
  { title: 'CityNo', dataIndex: 'CityNo', width: '10%' },
  { title: '市', dataIndex: 'City', width: '10%' },
  { title: 'AreaNo', dataIndex: 'AreaNo', width: '10%' },
  { title: '区', dataIndex: 'Area', width: '10%' },
  { title: '详细地址', dataIndex: 'Address', width: '10%' },
  { title: '收货人', dataIndex: 'Receiver', width: '10%' },
  { title: '收货人手机号', dataIndex: 'ReceiverPhone', width: '10%' },
  { title: '折扣', dataIndex: 'Discount', width: '10%' },
  { title: '总价', dataIndex: 'TotalPrice', width: '10%' },
  { title: '操作', dataIndex: 'action', scopedSlots: { customRender: 'action' } }
]

export default {
  components: {
    EditForm
  },
  mounted () {
    this.getDataList()
  },
  data () {
    return {
      data: [],
      pagination: {
        current: 1,
        pageSize: 10,
        showTotal: (total, range) => `总数:${total} 当前:${range[0]}-${range[1]}`
      },
      filters: {},
      sorter: { field: 'Id', order: 'asc' },
      loading: false,
      columns,
      queryParam: {},
      selectedRowKeys: []
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
        .post('/OrderAssistant/Order/GetDataList', {
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
    hanldleAdd () {
      this.$refs.editForm.openForm()
    },
    handleEdit (id) {
      this.$refs.editForm.openForm(id)
    },
    handleDelete (ids) {
      var thisObj = this
      this.$confirm({
        title: '确认删除吗?',
        onOk () {
          return new Promise((resolve, reject) => {
            thisObj.$http.post('/OrderAssistant/Order/DeleteData', ids).then(resJson => {
              resolve()

              if (resJson.Success) {
                thisObj.$message.success('操作成功!')

                thisObj.getDataList()
              } else {
                thisObj.$message.error(resJson.Msg)
              }
            })
          })
        }
      })
    }
  }
}
</script>
