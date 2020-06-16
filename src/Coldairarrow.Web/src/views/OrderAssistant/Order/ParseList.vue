<template>
  <div style="text-align: right">

    <a-button type="primary" class="editable-add-btn" :loading="loading" @click="exportExcel" >
      导出excel
    </a-button>
    <a-table :columns="columns" :data-source="data" :scroll="{ x: '200%' }" :loading="loading" bordered>
      <template
        v-for="col in ['Province','City','Area','Address','Receiver','ReceiverPhone','SkuNo','Count','Price']"
        :slot="col"
        slot-scope="text, record"
      >
        <div :key="col">
          <a-select
            v-if="record.editable && col==='SkuNo'"
            :default-value="record.SkuNo"
            show-search
            placeholder="请选择SKU编号"
            option-filter-prop="children"
            :filter-option="filterOption"
            @change="val => handleSkuChange(val, record.Id, col)"
            style="margin: -5px 0;width: 100%"
          >
            <a-select-option
              v-for="(item, index) in skuData"
              :key="index"
              :value="item.Id">
              {{ item.SkuNo }}
            </a-select-option>
          </a-select>
          <a-input
            v-else-if="record.editable"
            style="margin: -5px 0"
            :value="text"
            @change="e => handleChange(e.target.value, record.Id, col)"
          />
          <template v-else>
            {{ text }}
          </template>
        </div>
      </template>
      <template slot="operation" slot-scope="text, record">
        <div class="editable-row-operations">
          <span v-if="record.editable">
            <a @click="() => save(record.Id)">保存</a>
            <a-popconfirm title="确定取消?" @confirm="() => cancel(record.Id)">
              <a>取消</a>
            </a-popconfirm>
          </span>
          <span v-else>
            <a :disabled="editingKey !== ''" @click="() => edit(record.Id)">编辑</a>
            <a-popconfirm title="确定删除?" @confirm="() => onDelete(record.Id)">
              <a>删除</a>
            </a-popconfirm>
          </span>
        </div>
      </template>
    </a-table>
  </div>
</template>
<script>
import Excel from '@/utils/excel.js'

const columns = [
  {
    title: '原始编号',
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
    scopedSlots: { customRender: 'ReceiverName' }
  },
  {
    title: '收货人手机',
    width: '10%',
    dataIndex: 'ReceiverPhone',
    scopedSlots: { customRender: 'ReceiverPhone' }
  },
  {
    title: '商品编号/sku编号',
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
    width: '10%',
    scopedSlots: { customRender: 'operation' }
  }
]

export default {
  props: {
    loading: {
      type: Boolean,
      default: false
    }
  },
  mounted () {
    this.getSkuDataList()
  },
  data () {
    // const data = this.orders
    // this.setCacheData(data)
    return {
      data: [],
      skuData: [],
      columns,
      editingKey: ''
    }
  },
  computed: {
    listentCustomerId () {
      return this.$store.state.app.selectedCustomerId
    }
  },
  watch: {
    listentCustomerId: function (val) {
      this.getSkuDataList()
    }
  },
  methods: {
    setData (d) {
      this.data = d
      this.setCacheData(d)
    },
    getData () {
      return this.data
    },
    setCacheData (d) {
      this.cacheData = d.map(item => ({ ...item }))
    },
    handleChange (value, key, column) {
      const newData = [...this.data]
      const target = newData.filter(item => key === item.Id)[0]
      if (target) {
        target[column] = value
        this.data = newData
      }
    },
    handleSkuChange (value, key, column) {
      const newData = [...this.data]
      const target = newData.filter(item => key === item.Id)[0]

      const sku = this.skuData.filter(sd => sd.Id === value)[0]
      if (target) {
        target.SkuId = value
        target.SkuNo = sku.SkuNo
        target.SkuName = sku.SkuName
        target.Price = sku.Price
        this.data = newData
      }
    },
    exportExcel () {
      const excelFields = {
        '原始编号': 'OrderNo',
        '生成时间': 'CreateTime',
        '客户账号': 'CustomerNo',
        '省': 'Province',
        '市': 'City',
        '区': 'Area',
        '收货地址': 'Address',
        '收货人姓名': 'Receiver',
        '收货人手机': 'ReceiverPhone',
        '商品编号/sku编号': 'SkuNo',
        '数量': 'Count',
        '单价': 'Price'
      }
      Excel.exportExcel(this.data, excelFields, '订单')
    },
    onDelete (key) {
      const dataSource = [...this.data]
      const filterData = dataSource.filter(item => item.Id !== key)
      this.data = filterData
      this.setCacheData(filterData)
    },
    edit (key) {
      const newData = [...this.data]
      const target = newData.filter(item => key === item.Id)[0]
      this.editingKey = key
      if (target) {
        target.editable = true
        this.data = newData
      }
    },
    save (key) {
      const newData = [...this.data]
      const newCacheData = [...this.cacheData]
      const target = newData.filter(item => key === item.Id)[0]
      const targetCache = newCacheData.filter(item => key === item.Id)[0]
      if (target && targetCache) {
        delete target.editable
        this.data = newData
        Object.assign(targetCache, target)
        this.cacheData = newCacheData
      }
      this.editingKey = ''
    },
    cancelEdit () {
      this.cancel(this.editingKey)
    },
    cancel (key) {
      if (key === undefined || key == null || key.length <= 0) {
        return
      }
      const newData = [...this.data]
      const target = newData.filter(item => key === item.Id)[0]
      this.editingKey = ''
      if (target) {
        Object.assign(target, this.cacheData.filter(item => key === item.Id)[0])
        delete target.editable
        this.data = newData
      }
    },
    getSkuDataList () {
      var customerId = this.$store.state.app.selectedCustomerId
      this.$http
        .post('/OrderAssistant/Customer/GetSkuList', {
          CustomerId: customerId
        })
        .then(resJson => {
          this.skuData = resJson.Data
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
<style scoped>
.editable-row-operations a {
  margin-right: 8px
}
.editable-add-btn {
  margin-bottom: 8px
}
</style>
