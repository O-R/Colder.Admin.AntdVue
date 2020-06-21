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
      <a-button type="primary" @click="hanldeDownloadTemplate()" :loading="loading" >
        模板下载
      </a-button>
      <a-button type="primary" @click="hanldleImport()" :loading="loading" >
        导入Excel
      </a-button>
    </div>

    <div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="10">
          <a-col :md="4" :sm="24">
            <a-form-item label="查询类别">
              <a-select allowClear v-model="queryParam.condition">
                <a-select-option key="SkuNo">sku编号</a-select-option>
                <a-select-option key="SkuName">sku名称</a-select-option>
                <a-select-option key="KeyWords">关键词</a-select-option>
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
import Excel from '@/utils/excel.js'

const columns = [
  { title: 'SKU编号', dataIndex: 'SkuNo', width: '10%' },
  { title: 'SKU名称', dataIndex: 'SkuName', width: '10%' },
  { title: '关键词', dataIndex: 'KeyWords', width: '10%' },
  { title: '操作', dataIndex: 'action', scopedSlots: { customRender: 'action' } }
]

export default {
  components: {
    EditForm
  },
  mounted () {
    this.getDataList()
    this.getCustomerList()
  },
  data () {
    return {
      data: [],
      pagination: {
        current: 1,
        pageSize: 20,
        showTotal: (total, range) => `总数:${total} 当前:${range[0]}-${range[1]}`
      },
      filters: {},
      sorter: { field: 'Id', order: 'asc' },
      loading: false,
      columns,
      queryParam: {},
      selectedRowKeys: [],
      customerData: []
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
        .post('/OrderAssistant/Sku/GetDataList', {
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
      this.$refs.editForm.openForm(null, '新增')
    },
    handleEdit (id) {
      this.$refs.editForm.openForm(id, '编辑')
    },
    handleDelete (ids) {
      var thisObj = this
      this.$confirm({
        title: '确认删除吗?',
        onOk () {
          return new Promise((resolve, reject) => {
            thisObj.$http.post('/OrderAssistant/Sku/DeleteData', ids).then(resJson => {
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
    },
    getCustomerList () {
      this.$http
        .post('/OrderAssistant/Customer/GetDataList', {
          PageIndex: 1,
          SortField: 'Id',
          SortType: 'asc',
          Search: {}
        })
        .then(resJson => {
          this.customerData = resJson.Data
        })
    },
    hanldeDownloadTemplate () {
      const excelFields = {
        '商品编码': '',
        '商品名称': '',
        '关键词1': '',
        '关键词2': '',
        '关键词3': '',
        '关键词4': '',
        '关键词5': '',
        '关键词6': '',
        '关键词7': '',
        '关键词8': '',
        '关键词9': '',
        '关键词10': '',
        '关键词11': '',
        '关键词12': '',
        '关键词13': '',
        '关键词14': '',
        '关键词15': ''
      }
      Excel.exportExcel([], excelFields, 'SKU导入模板')
    },
    hanldleImport () {
      var that = this
      Excel.importExcel((data, dataRef) => {
        if (data.length < 2) {
          that.$message.success('导入数据为空!')
        } else {
          this.loading = true
          var list = []

          data.map((d, idx) => {
            if (idx > 0) {
              var row = { SkuNo: d[0], SkuName: d[1], KeyWords: '' }

              var keyArr = []
              for (let i = 2; i < 15; i++) {
                if (d[i] && d[i].toString().trim()) {
                  keyArr.push(d[i].toString().trim())
                }
              }
              row.KeyWords = keyArr.toString()
              list.push(row)
            }
          })

          this.$http
            .post('/OrderAssistant/Sku/Import', list)
            .then(resJson => {
              this.loading = false

              if (resJson.Success) {
                this.$message.success('导入成功!')
                this.getDataList()
              } else {
                this.$message.error('导入异常!')
              }
            })
        }
      })
    }
  }
}
</script>
