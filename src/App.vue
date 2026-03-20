<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { initFlowbite } from 'flowbite'
import LeftNav from '@/components/LeftNav.vue'
import Header from '@/components/Header.vue'

const isSidebarOpen = ref<boolean>(false)

onMounted(() => {
  initFlowbite()
})
</script>

<template>
  <div class="antialiased bg-gray-50 dark:bg-gray-900">
    <div v-if="$route.meta.layout === 'dashboard'">
      <Header @toggle-sidebar="isSidebarOpen = !isSidebarOpen"/>
      <LeftNav :is-open="isSidebarOpen"/>
      <main class="p-4 md:ml-64 pt-20 h-screen overflow-y-auto">
        <router-view/>
      </main>
    </div>
    <div v-else-if="$route.meta.layout === 'none'">
      <main class="h-screen overflow-y-auto">
        <router-view/>
      </main>
    </div>
  </div>
</template>
