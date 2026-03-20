<script setup lang="ts">
defineProps<{
  menu: string
}>()
</script>

<template>
  <nav class="flex" aria-label="Breadcrumb">
    <ol class="inline-flex items-center space-x-1 md:space-x-2 rtl:space-x-reverse">
      <li class="inline-flex items-center">
        <router-link to="/" class="inline-flex items-center text-sm font-medium text-body hover:text-fg-brand">
          <i class="fa fa-home"></i>
        </router-link>
      </li>
      <li>
        <div class="flex items-center space-x-1.5">
          <i class="fa fa-chevron-right text-sm text-body-subtle"></i>
          <span class="inline-flex items-center text-sm font-medium text-body-subtle">{{ menu }}</span>
        </div>
      </li>
    </ol>
  </nav>
</template>

<style scoped>

</style>
