<template>
  <div style="text-align: right">
    <a-button type="primary" class="import-btn" @click="hanldeDownloadTemplate()" :loading="loading" >
      模板下载
    </a-button>
    <a-button type="primary" class="import-btn" @click="hanldleImport()" :loading="loading" >
      导入Excel
    </a-button>
    <a-button type="primary" class="editable-add-btn" @click="hanldleAdd()" :loading="loading" >
      新增行
    </a-button>
    <a-table :columns="columns" :data-source="data" :loading="loading" :pagination="pagination" bordered>
      <template
        v-for="col in ['address','skus']"
        :slot="col"
        slot-scope="text, record"
      >
        <div :key="col">
          <a-input
            v-if="record.editable"
            style="margin: -5px 0"
            :value="text"
            @change="e => handleChange(e.target.value, record.key, col)"
          />
          <template v-else>
            {{ text }}
          </template>
        </div>
      </template>
      <template slot="operation" slot-scope="text, record">
        <div class="editable-row-operations">
          <span v-if="record.editable">
            <a @click="() => save(record.key)">保存</a>
            <a-popconfirm title="确定取消?" @confirm="() => cancel(record.key)">
              <a>取消</a>
            </a-popconfirm>
          </span>
          <span v-else>
            <a :disabled="editingKey !== ''" @click="() => edit(record.key)">编辑</a>
            <a-popconfirm title="确定删除?" @confirm="() => onDelete(record.key)">
              <a>删除</a>
            </a-popconfirm>
          </span>
        </div>
      </template>
    </a-table>

    <import-form ref="importForm" :parentObj="this"></import-form>
  </div>
</template>
<script>

import Excel from '@/utils/excel.js'
// import XLSX from 'xlsx'
import ImportForm from './ImportForm'

const columns = [
  {
    title: '序号',
    dataIndex: 'idx',
    width: '8%',
    scopedSlots: { customRender: 'idx' }
  },
  {
    title: '地址',
    dataIndex: 'address',
    width: '40%',
    scopedSlots: { customRender: 'address' }
  },
  {
    title: '型号',
    dataIndex: 'skus',
    width: '40%',
    scopedSlots: { customRender: 'skus' }
  },
  {
    title: '操作',
    dataIndex: 'operation',
    scopedSlots: { customRender: 'operation' }
  }
]

const data = []

export default {
  components: {
    ImportForm
  },
  props: {
    loading: {
      type: Boolean,
      default: false
    }
  },
  data () {
    this.setCacheData(data)
    return {
      data,
      columns,
      editingKey: '',
      pagination: {
        current: 1,
        pageSize: 20,
        showTotal: (total, range) => `总数:${total} 当前:${range[0]}-${range[1]}`,
        showSizeChanger: true
      }
    }
  },
  methods: {
    setCacheData (d) {
      this.cacheData = d.map(item => ({ ...item }))
    },
    handleChange (value, key, column) {
      const newData = [...this.data]
      const target = newData.filter(item => key === item.key)[0]
      if (target) {
        target[column] = value
        this.data = newData
      }
    },
    getParseData () {
      var that = this
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
        fullAddress: '',
        isError: false,
        errorMessage: ''
      }
      return this.data.map(item => {
        var addr = {}
        try {
          addr = that.smartParse(item.address)
        } catch (error) {
          addr = Object.assign(addr, defaultItem)
          addr.isError = true
          addr.address = item.address
          addr.errorMessage = '地址解析异常'
        }
        addr.skuKeyWords = item.skus
        addr.fullAddress = item.address
        addr.index = item.key

        if (!addr.isError && (!addr.province || !addr.city || !addr.county || !addr.address || !addr.name || !addr.phone)) {
          addr.isError = true
          addr.address = item.address
          addr.errorMessage = '地址解析异常'
        }

        return addr
      })
    },
    onDelete (key) {
      const dataSource = [...this.data]
      const filterData = dataSource.filter(item => item.key !== key)
      this.data = filterData
      this.setCacheData(filterData)
    },
    hanldleAdd () {
      this.$refs.importForm.openForm('新增')
    },
    hanldeDownloadTemplate () {
      const excelFields = {
        '地址': 'address',
        '型号': 'skus'
      }
      Excel.exportExcel([], excelFields, '订单录入信息模板')
    },
    hanldleImport () {
      var that = this
      Excel.importExcel((data, dataRef) => {
        if (data.length < 2) {
          that.$message.success('导入数据为空!')
        } else {
          data.map((d, idx) => {
            if (idx > 0) {
              that.add({ address: d[0], skus: d[1] })
            }
          })
        }
      })
    },
    add (row) {
      const { data } = this
      var i = data.length > 0 ? data[data.length - 1].idx : 0
      row.idx = i + 1
      row.key = i.toString()
      const addData = [...data, row]
      this.data = addData
      this.setCacheData(addData)
    },
    edit (key) {
      const newData = [...this.data]
      const target = newData.filter(item => key === item.key)[0]
      this.editingKey = key
      if (target) {
        target.editable = true
        this.data = newData
      }
    },
    save (key) {
      const newData = [...this.data]
      const newCacheData = [...this.cacheData]
      const target = newData.filter(item => key === item.key)[0]
      if (target.address.toString().trim() === '') {
        this.$message.warn('地址必填')
        return
      }

      if (target.skus.toString().trim() === '') {
        this.$message.warn('型号必填')
        return
      }
      const targetCache = newCacheData.filter(item => key === item.key)[0]
      if (target && targetCache) {
        delete target.editable
        this.data = newData
        Object.assign(targetCache, target)
        this.cacheData = newCacheData
      }
      this.editingKey = ''
    },
    cancel (key) {
      const newData = [...this.data]
      const target = newData.filter(item => key === item.key)[0]
      this.editingKey = ''
      if (target) {
        Object.assign(target, this.cacheData.filter(item => key === item.key)[0])
        delete target.editable
        this.data = newData
      }
    }
  }
}
</script>
<style scoped>
.editable-row-operations a {
  margin-right: 8px
}
.editable-add-btn {
  margin-bottom: 8px
}
.import-btn {
  margin-bottom: 8px;
  margin-right: 20px;
}
</style>
