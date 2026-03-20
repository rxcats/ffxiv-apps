import axios from 'axios'

const client = axios.create({
  withCredentials: true,
})

client.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      // 로그인 만료 처리
      // router.push('/login')
    }
    return error
  }
)

export default client
