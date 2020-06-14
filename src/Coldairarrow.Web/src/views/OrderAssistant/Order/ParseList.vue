<template>
  <div style="text-align: right">
    <a-button type="primary" class="editable-add-btn" @click="hanldleAdd()" >
      编辑
    </a-button>
    <a-table :columns="columns" :data-source="data" :scroll="{ x: '200%' }" :loading="loading" bordered>
      <template
        v-for="col in ['Province','City','Area','Address','Receiver','ReceiverPhone','SkuNo','Count','Price']"
        :slot="col"
        slot-scope="text, record"
      >
        <div :key="col">
          <a-input
            v-if="record.editable"
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
  data () {
    // const data = this.orders
    // this.setCacheData(data)
    return {
      data: [],
      columns,
      editingKey: '',
      loading: false
    }
  },
  methods: {
    setData (d) {
      this.data = d
      this.setCacheData(d)
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
    onDelete (key) {
      const dataSource = [...this.data]
      const filterData = dataSource.filter(item => item.Id !== key)
      this.data = filterData
      this.setCacheData(filterData)
    },
    hanldleAdd () {
    //   this.$refs.importForm.openForm('新增')
    },
    add (row) {
    //   const { data } = this
    //   var i = data[data.length - 1].idx
    //   row.idx = i + 1
    //   row.key = i.toString()
    //   const addData = [...data, row]
    //   this.data = addData
    //   this.setCacheData(addData)
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
    cancel (key) {
      const newData = [...this.data]
      const target = newData.filter(item => key === item.Id)[0]
      this.editingKey = ''
      if (target) {
        Object.assign(target, this.cacheData.filter(item => key === item.Id)[0])
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
</style>
