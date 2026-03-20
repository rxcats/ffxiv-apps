<script setup lang="ts">
defineProps<{
  isOpen: boolean
}>()
</script>

<template>
  <aside
    id="sidebar"
    :class="[
      'fixed top-0 left-0 z-40 w-64 h-screen pt-14 transition-transform bg-white border-r border-gray-200 dark:bg-gray-800 dark:border-gray-700',
      isOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'
    ]"
    aria-label="Sidenav"
  >
    <div class="overflow-y-auto py-5 px-3 h-full bg-white dark:bg-gray-800">
      <ul class="space-y-2">
        <li>
          <router-link to="/"  class="flex items-center p-2 text-base font-medium text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700 group">
            <i class="fa fa-home"></i>
            <span class="ml-3">Home</span>
          </router-link>
        </li>
        <li>
          <router-link to="/macro"  class="flex items-center p-2 text-base font-medium text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700 group">
            <i class="fa-solid fa-computer-mouse"></i>
            <span class="ml-3">FF14 매크로</span>
          </router-link>
        </li>
        <li>
          <router-link to="/housekeeper"  class="flex items-center p-2 text-base font-medium text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700 group">
            <i class="fa-solid fa-gavel"></i>
            <span class="ml-3">FF14 집사장비</span>
          </router-link>
        </li>
        <li>
          <router-link to="/link-collection"  class="flex items-center p-2 text-base font-medium text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700 group">
            <i class="fa-solid fa-link"></i>
            <span class="ml-3">FF14 링크모음</span>
          </router-link>
        </li>
        <li>
          <router-link to="/submarine"  class="flex items-center p-2 text-base font-medium text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700 group">
            <i class="fa fa-gamepad"></i>
            <span class="ml-3">FF14 고잉마샤호</span>
          </router-link>
        </li>
      </ul>
    </div>
  </aside>
</template>

<style scoped>

</style>
