<template>
    <div class="throug-add">
        <BtnBox @cancel="Cancel" @sure="Sure"></BtnBox>
    </div>
</template>
        
<script>
import BtnBox from '@/components/Btns'
import { mapActions, mapState } from 'vuex'
export default {
  name: 'througadd',
  data () {
    return {
    }
  },
  components: {
    BtnBox
  },
  computed: {
    ...mapState('System', ['OpenGate'])
  },
  methods: {
    ...mapActions('System', ['AddNewOpenGateReason', 'ModifyOpenGateReason']),
    Cancel () {
      this.$router.push('/Through')
    },
    async Sure () {

    }
  }
}
</script>

<style>

</style>
          