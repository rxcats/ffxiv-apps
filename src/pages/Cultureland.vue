<script setup lang="ts">
import axios from 'axios'
import { onMounted, ref } from 'vue'
import Breadcrumb from '@/components/Breadcrumb.vue'
import { DataTable } from 'simple-datatables'
import 'reflect-metadata'
import { plainToInstance } from 'class-transformer'
import LoadingOverlay from '@/components/LoadingOverlay.vue'

function formattedPrice(price: number) {
  return new Intl.NumberFormat('ko-KR').format(price)
}

class GiftPrice {
  id: number = 0;
  giftType: string = "";
  price: number = 0;
  discountRate: number = 0.0;
  market: string = "";
  goods: string = "";
  goodsLink: string = "";

  // createdAt: string = "";

  get formattedPrice(): string {
    return formattedPrice(this.price)
  }

  get formattedRate(): string {
    return this.discountRate + '%'
  }

  get href(): string {
    return `<a href="${this.goodsLink}" target="_blank">${this.goods}</a>`
  }

  toDataTableData(): any {
    return {
      giftType: this.giftType,
      price: this.formattedPrice,
      discountRate: this.formattedRate,
      market: this.market,
      goods: this.href,
    }
  }
}

const isLoading = ref<boolean>(false)
const dataTable = ref<DataTable>()

const options = {
  searchable: false,
  paging: false,
  labels: {
    placeholder: '검색어',
    info: '{start} - {end} (총 {rows} 개)',
    noRows: '텅!',
    noResults: '텅!',
    perPage: '개',
  },
  data: {
    headings: [
      { data: 'giftType', text: '종류' },
      { data: 'price', text: '가격' },
      { data: 'discountRate', text: '할인' },
      { data: 'market', text: '상점' },
      { data: 'goods', text: '상품' },
    ] as any[],
    data: []
  },
}

function triggerLoading() {
  if (isLoading.value) return

  isLoading.value = true
}

function closeLoading() {
  if (!isLoading.value) return

  setTimeout(() => {
    isLoading.value = false
  }, 300)
}

function apiUrl(path: string): string {
  return import.meta.env.VITE_FF14_APPS_API_URL + path
}

async function fetchGiftPriceData() {
  triggerLoading()

  if (!dataTable.value) {
    dataTable.value = new DataTable('#dataTable', options)
  }

  const url = apiUrl(`/api/gift/items`)

  try {
    const response = await axios.get(url)
    if (response.status === 200 && response.data) {
      let data: any[] = []

      const giftPriceItems = plainToInstance<GiftPrice, any[]>(GiftPrice, response.data)

      for (let item of giftPriceItems) {
        data.push(item.toDataTableData())
      }

      dataTable.value.data.data = []
      dataTable.value.insert(data)
    }
  } catch (e) {
    console.error(e)
  } finally {
    closeLoading()
  }
}

onMounted(() => {
  fetchGiftPriceData()
})
</script>

<template>
  <Breadcrumb menu="컬쳐랜드 상품권"/>

  <div class="mt-5"></div>

  <div class="bg-white dark:bg-gray-900">
    <div class="col-span-1 space-x-1.5 flex">
      <button type="button" @click="fetchGiftPriceData"
              class="inline-flex items-center justify-center text-white bg-brand hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium shadow-xs font-medium rounded-base text-xs px-3 py-2.5 focus:outline-none cursor-pointer">
        <i class="fa fa-recycle mr-1"></i> 새로고침
      </button>
    </div>

    <table id="dataTable" class="w-full text-sm text-left rtl:text-right text-body"></table>

    <!-- Loading -->
    <Teleport to="body">
      <LoadingOverlay :isLoading="isLoading"/>
    </Teleport>
  </div>
</template>

<style scoped>

</style>
