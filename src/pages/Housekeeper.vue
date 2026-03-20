<script setup lang="ts">
import Breadcrumb from '@/components/Breadcrumb.vue'

const battleHeader = ['레벨(아이템레벨) / 획득량', '5개', '7개', '10개', '12개', '15개']
const battleData = [
  [95, 646, 661, 676, 681, 691],
  [96, 649, 664, 679, 684, 694],
  [97, 652, 667, 682, 687, 697],
  [98, 655, 670, 685, 690, 700],
  [99, 658, 673, 688, 693, 703],
  [100, 670, 685, 700, 705, 715],
]

const craftHeader = ['레벨(기술력) / 획득량', '20개', '30개']
const craftData = [
  [95, 3224, 4605],
  [96, 3308, 4725],
  [97, 3392, 4845],
  [98, 3434, 4905],
  [99, 3434, 4905],
  [100, 3476, 4965],
]
</script>

<template>
  <Breadcrumb menu="FF14 집사장비"/>

  <div class="mt-5"></div>

  <p class="text-2xl text-heading">전투 집사</p>
  <div class="mt-3 relative overflow-x-auto bg-neutral-primary-soft shadow-xs rounded-base border border-default">
    <table class="w-full text-sm text-left rtl:text-right text-body">
      <thead class="text-sm text-body bg-neutral-secondary-soft border-b rounded-base border-default">
      <tr>
        <th v-for="h in battleHeader" scope="col" class="px-6 py-3 font-medium">
          {{ h }}
        </th>
      </tr>
      </thead>
      <tbody>
      <tr v-for="rows in battleData" class="bg-neutral-primary border-b border-default">
        <td v-for="col in rows" class="px-6 py-4">
          {{ col }}
        </td>
      </tr>
      </tbody>
    </table>
  </div>

  <div class="m-5"></div>

  <p class="text-2xl text-heading">채집 집사</p>
  <div class="mt-3 relative overflow-x-auto bg-neutral-primary-soft shadow-xs rounded-base border border-default">
    <table class="w-full text-sm text-left rtl:text-right text-body">
      <thead class="text-sm text-body bg-neutral-secondary-soft border-b rounded-base border-default">
      <tr>
        <th v-for="h in craftHeader" scope="col" class="px-6 py-3 font-medium">
          {{ h }}
        </th>
      </tr>
      </thead>
      <tbody>
      <tr v-for="rows in craftData" class="bg-neutral-primary border-b border-default">
        <td v-for="col in rows" class="px-6 py-4">
          {{ col }}
        </td>
      </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>

</style>
