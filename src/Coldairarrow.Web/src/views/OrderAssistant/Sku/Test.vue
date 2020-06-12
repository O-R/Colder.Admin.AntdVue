
<!-- <a-steps :current="1" size="small"> -->

<template>
  <a-steps :current="step">
    <a-step title="请选择下单客户" />
    <a-step title="请录入信息" />
    <a-step title="完成" />
  </a-steps>
</template>

<script>
export default {

  data () {
    return {
      step: 0
    }
  }
}
</script>
