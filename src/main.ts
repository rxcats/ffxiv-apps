import { createApp } from 'vue'
import '@/style.css'
import App from '@/App.vue'
import router from '@/router'
import { numberOnly } from '@/directives/numberOnly'
import { dateFormat } from '@/directives/dateFormat'

const app = createApp(App)
app.use(router)
app.directive('number-only', numberOnly)
app.directive('date-format', dateFormat)
app.mount('#app')
