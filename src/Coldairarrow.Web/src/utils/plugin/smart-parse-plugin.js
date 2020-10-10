import smart from '../../components/tools/SmartParse.vue'

export default {
  install (Vue) {
    Object.defineProperty(Vue.prototype, '$smartParse', { value: smart.smart })
  }
}
