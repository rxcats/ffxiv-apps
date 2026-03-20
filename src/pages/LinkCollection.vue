<script setup lang="ts">
import Breadcrumb from '@/components/Breadcrumb.vue'

const links = [
  {
    name: '갈론드벨',
    url: 'https://garlandtools.org/bell',
    icon: 'https://garlandtools.org/favicon.png',
  },
  {
    name: '고양이는 배고프다',
    url: 'https://ko.ff14angler.com',
    icon: 'https://ko.ff14angler.com/favicon.ico',
  },
  {
    name: '잠수함과 심해속으로',
    url: 'https://submarine.girin.dev',
    icon: 'https://submarine.girin.dev/favicon.ico',
  },
  {
    name: '롱카의 옷장',
    url: 'https://ronkacloset.com',
    icon: 'https://cdn.imweb.me/thumbnail/20220508/5bdf0e90c6a43.png',
  },
]

</script>

<template>
  <Breadcrumb menu="FF14 링크모음"/>

  <div class="mt-5"></div>

  <p class="text-2xl text-heading">링크모음</p>
  <div class="mt-3 w-96 bg-neutral-primary-soft border border-default rounded-base shadow-xs">
    <ul role="list" class="space-y-3 p-6 divide-y divide-default">
      <li v-for="link in links" class="flex items-center justify-between pb-2">
        <div class="flex items-center text-body">
          <img :src="link.icon" class="w-5 h-5 mr-3" alt=""/>
          <a :href="link.url" target="_blank"><span>{{ link.name }}</span>
          </a>
        </div>
      </li>
    </ul>
  </div>

</template>

<style scoped>

</style>
