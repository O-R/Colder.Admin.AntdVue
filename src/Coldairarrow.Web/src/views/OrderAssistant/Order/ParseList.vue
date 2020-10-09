<template>
  <div style="text-align: right">

    <a-button type="primary" class="editable-add-btn" :loading="loading" @click="exportExcel" >
      导出excel
    </a-button>
    <a-table
      :columns="columns"
      :data-source="data"
      :scroll="{ x: '200%' }"
      :loading="loading"
      :rowClassName="setRowClassName"
      :pagination="pagination"
      @change="handleTableChange"
      bordered >
      <template
        v-for="col in ['Province','City','Area','Address','Receiver','ReceiverPhone','SkuName','SkuNo','Count','Price']"
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
          <a-input-number
            v-else-if="record.editable && col==='Count'"
            v-model="record.Count"
            style="margin: -5px 0"
            :min="1"
            :precision="0"
            @change="num => handleChange(num, record.Id, col)"
          />
          <a-input-number
            v-else-if="record.editable && col==='Price'"
            v-model="record.Price"
            style="margin: -5px 0"
            :min="0"
            :precision="2"
            @change="num => handleChange(num, record.Id, col)"
          />
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
            <a v-if="record.IsError" @click="() => saveAsSuccess(record.Id)">保存为正确数据</a>
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
    <a-modal v-model="visible" title="拆词结果" on-ok="handleKnowResult">
      <template slot="footer">
        <a-button key="back" type="primary" @click="handleKnowResult">
          我知道了
        </a-button>
      </template>
      <p>拆词完成，{{ SuccessRowCount }} 条数据成功，{{ ErrorRowCount }} 条数据失败</p>
    </a-modal>
  </div>
</template>
<script>
import Excel from '@/utils/excel.js'
import moment from 'moment'

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
      editingKey: '',
      SuccessRowCount: 0,
      ErrorRowCount: 0,
      visible: false,
      pagination: {
        current: 1,
        pageSize: 40,
        showTotal: (total, range) => `总数:${total} 当前:${range[0]}-${range[1]}`,
        showSizeChanger: true,
        pageSizeOptions: ['20', '40', '60', '80', '100']
      }
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

      this.ErrorRowCount = this.data.reduce(function (sum, item) {
        return sum + (item.IsError ? 1 : 0)
      }, 0)

      this.SuccessRowCount = this.data.length - this.ErrorRowCount
      this.visible = true
    },
    getData () {
      return this.data
    },
    setCacheData (d) {
      this.cacheData = d.map(item => ({ ...item }))
    },
    setRowClassName (record, index) {
      return record.IsError ? 'errorRow-highlight' : ''
    },
    handleKnowResult () {
      this.visible = false
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
        // Object.assign(this.data, newData)
      }
    },
    handleTableChange (pagination, filters, sorter) {
      this.pagination = { ...pagination }
      this.filters = { ...filters }
      this.sorter = { ...sorter }
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
      var fileId = this.data[0].CustomerName + '-' + t
      Excel.exportExcel(this.data, excelFields, fileId)
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

      const targeItems = newData.filter(item => key !== item.Id && item.OrderNo === target.OrderNo)
      const targetCacheItems = newCacheData.filter(item => key !== item.Id && item.OrderNo === target.OrderNo)

      if (targeItems && targeItems.length > 0 && targetCacheItems && targetCacheItems.length > 0) {
        targeItems.map(it => {
          it.Province = target.Province
          it.City = target.City
          it.Area = target.Area
          it.Address = target.Address
          it.Receiver = target.Receiver
          it.ReceiverPhone = target.ReceiverPhone
        })

        targetCacheItems.map(it => {
          it.Province = target.Province
          it.City = target.City
          it.Area = target.Area
          it.Address = target.Address
          it.Receiver = target.Receiver
          it.ReceiverPhone = target.ReceiverPhone
        })
      }

      this.editingKey = ''
    },
    saveAsSuccess (key) {
      const newData = [...this.data]
      const newCacheData = [...this.cacheData]
      const target = newData.filter(item => key === item.Id)[0]

      target.IsError = false

      const targetCache = newCacheData.filter(item => key === item.Id)[0]
      if (target && targetCache) {
        delete target.editable
        this.data = newData
        Object.assign(targetCache, target)
        this.cacheData = newCacheData
      }
      const targeItems = newData.filter(item => key !== item.Id && item.OrderNo === target.OrderNo)
      const targetCacheItems = newCacheData.filter(item => key !== item.Id && item.OrderNo === target.OrderNo)

      if (targeItems && targeItems.length > 0 && targetCacheItems && targetCacheItems.length > 0) {
        targeItems.map(it => {
          it.Province = target.Province
          it.City = target.City
          it.Area = target.Area
          it.Address = target.Address
          it.Receiver = target.Receiver
          it.ReceiverPhone = target.ReceiverPhone
        })

        targetCacheItems.map(it => {
          it.Province = target.Province
          it.City = target.City
          it.Area = target.Area
          it.Address = target.Address
          it.Receiver = target.Receiver
          it.ReceiverPhone = target.ReceiverPhone
        })
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

<style>
.errorRow-highlight {
  background-color: #f0f9eb;
}
</style>
