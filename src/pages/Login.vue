<script setup lang="ts">
import { onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const baseUrl =import.meta.env.VITE_FF14_APPS_API_URL

const httpClient = axios.create({
  baseURL: baseUrl,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/x-www-form-urlencoded'
  },
})

const router = useRouter()

const form = reactive({ username: '', password: '' })

function resetForm() {
  form.username = ''
  form.password = ''
}

async function loginHandler() {
  try {
    const response = await httpClient.post('/api/auth/login', form)
    console.log('response', response)
    if (response.status === 200) {
      resetForm()
      await router.push('/login')
    }
  } catch (e) {
    console.log('로그인 실패: ', e)
  }
}

async function checkLogin() {
  try {
    const response = await httpClient.get('/api/auth/profile')
    
    console.log('profile data', response.data)
    console.log('profile headers', response.headers)
  } catch (e) {
    console.log('실패: ' + e)
  }
}

onMounted(() => {
  checkLogin()
})

// async function logoutHandler() {
//   try {
//     const response = await api.post(apiUrl('/api/auth/logout'), null, {
//       headers: {
//         "Content-Type": "application/x-www-form-urlencoded"
//       }
//     })
//     console.log('data', response.data)
//     console.log('headers', response.headers)
//
//     await getProfile()
//   } catch (e) {
//     console.log('로그아웃 실패: ', e)
//   }
// }
</script>

<template>
  <div class="flex items-center justify-center h-screen">
    <div class="bg-neutral-primary-soft block max-w-sm p-6 border border-default rounded-base shadow-xs">
      <h5 class="text-center mb-3 text-2xl font-semibold tracking-tight text-heading leading-8">FF14 Apps</h5>

      <form @submit.prevent="loginHandler">
        <div class="mb-5">
          <label for="" class="block mb-2.5 text-sm font-medium text-heading">아이디</label>
          <input type="text" v-model="form.username"
                 class="bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow placeholder:text-body"
                 placeholder="아이디" required/>
        </div>
        <div class="mb-5">
          <label for="" class="block mb-2.5 text-sm font-medium text-heading">암호</label>
          <input type="password" v-model="form.password"
                 class="bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow placeholder:text-body"
                 placeholder="******" required/>
        </div>

        <button type="submit"
                class="text-white bg-brand box-border border border-transparent hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium shadow-xs font-medium leading-5 rounded-base text-sm w-full px-3 py-2.5 focus:outline-none">
          로그인
        </button>
        
      </form>
    </div>

  </div>
</template>

<style scoped>

</style>
