import { createRouter, createWebHashHistory } from 'vue-router'

import Home from '@/pages/Home.vue'
import ErrorPage from '@/pages/ErrorPage.vue'
import Submarine from '@/pages/Submarine.vue'
import Housekeeper from '@/pages/Housekeeper.vue'
import MacroCalc from '@/pages/MacroCalc.vue'
import LinkCollection from '@/pages/LinkCollection.vue'
import Login from '@/pages/Login.vue'

const routes = [
  { path: '/login', component: Login, name: 'Login', meta: { layout: 'none' } },
  { path: '/', component: Home, name: 'Home', meta: { layout: 'dashboard' } },
  { path: '/macro', component: MacroCalc, name: 'MacroCalc', meta: { layout: 'dashboard' } },
  { path: '/housekeeper', component: Housekeeper, name: 'Housekeeper', meta: { layout: 'dashboard' } },
  { path: '/link-collection', component: LinkCollection, name: 'LinkCollection', meta: { layout: 'dashboard' } },
  { path: '/submarine', component: Submarine, name: 'Submarine', meta: { layout: 'dashboard' } },
  { path: '/:pathMatch(.*)*', component: ErrorPage, name: 'ErrorPage', meta: { layout: 'none' } },
]

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: routes
})

export default router
