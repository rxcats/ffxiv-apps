<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import axios from 'axios'
import 'reflect-metadata'
import { DataTable } from 'simple-datatables'
import { plainToInstance } from 'class-transformer'
import { DateTime } from 'luxon'
import fallbackImg from '@/assets/fallback.png'
import Toast from '@/components/Toast.vue'
import LoadingOverlay from '@/components/LoadingOverlay.vue'
import Breadcrumb from '@/components/Breadcrumb.vue'

function messageFromError(e: unknown) {
  if (e instanceof Error) {
    return e.message
  }
  return 'unknown error'
}

function apiUrl(path: string): string {
  return import.meta.env.VITE_FF14_APPS_API_URL + path
}

function parseItemIconUrl(itemId: number, itemIcon: number): string {
  const itemIconPad = itemIcon.toString().padStart(6, '0')

  const charArray: string[] = itemIconPad.split('')

  for (let i = 3; i < charArray.length; i++) {
    charArray[i] = '0'
  }

  const group = charArray.join('')

  if (itemId === 0) {
    return 'fallback.png'
  } else {
    return `https://ff14.tar.to/assets/img/icon/${group}/${itemIconPad}.tex.png`
  }
}

function formattedPrice(price: number) {
  return new Intl.NumberFormat('ko-KR').format(price)
}

class SoldItem {
  id: number = 0;
  itemName: string = '';
  itemId: number = 0;
  itemIcon: number = 0;
  quantity: number = 0;
  price: number = 0;
  soldDate: string = '';

  get formattedPrice(): string {
    return formattedPrice(this.price)
  }

  get itemNameTag(): string {
    return `<div class="flex items-center gap-2">${this.itemIconUrl} ${this.itemName} ✕ <span class="bg-brand-softer text-fg-brand-strong text-xs font-medium px-1.5 py-0.5 rounded">${this.quantity}</span></div>`
  }

  private get itemIconUrl(): string {
    // noinspection HtmlDeprecatedAttribute
    return `<img src="${parseItemIconUrl(this.itemId, this.itemIcon)}" onerror="this.onerror=null; this.src='fallback.png'" alt="" class="dark:bg-white w-10 h-10 rounded-sm" />`
  }

  toDataTableData(): any {
    return {
      id: this.id,
      itemName: this.itemNameTag,
      price: this.formattedPrice,
      soldDate: this.soldDate,
      actions: this.id,
    }
  }
}

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
      { data: 'actions', text: '선택' },
      { data: 'itemName', text: '아이템' },
      { data: 'price', text: '판매가격' },
      { data: 'soldDate', text: '판매일' },
    ] as any[],
    data: []
  },
  columns: [
    {
      select: 0,
      render: function (data: any) {
        return `<input type="checkbox" class="chk w-4 h-4 border border-default-medium rounded-sm bg-neutral-secondary-medium focus:ring-2 focus:ring-brand-soft" data-id="${data[0].data}">`
      }
    }
  ]
}

interface SoldItemInput {
  itemId: number;
  itemIcon: number;
  itemName: string;
  quantity: number;
  price: number;
  soldDate: string;
  url: string;
}

interface SoldItemInputReqBody {
  itemId: number;
  itemIcon: number;
  itemName: string;
  quantity: number;
  price: number;
  soldDate: string;
}

interface SoldItemBulkInput {
  text: string;
}

interface SearchReqBody {
  startDate: string;
  endDate: string;
}

interface ItemDetail {
  id: number;
  icon: number;
  name: string;
  level: number;
}

interface SoldPriceSum {
  yearMonth: string;
  priceSum: number;
}

const now = DateTime.now()

const dataTable = ref<DataTable>()
const checkedIds = ref<number[]>([])
const isDeleteModalOpen = ref<boolean>(false)
const isInputModalOpen = ref<boolean>(false)
const isChatInputModalOpen = ref<boolean>(false)
const soldItemInput = reactive<SoldItemInput>({
  itemId: 0,
  itemIcon: 0,
  itemName: '',
  quantity: 0,
  price: 0,
  soldDate: '',
  url: fallbackImg
})
const soldItemBulkInput = reactive<SoldItemBulkInput>({ text: '' })
const searchYear = ref<number>(now.year)
const searchMonth = ref<number>(now.month)
const showToast = ref<boolean>(false)
const toastMsg = ref<string>('')
const toastType = ref<'success' | 'error'>('success')
const isLoading = ref<boolean>(false)
const isSearchItem = ref<boolean>(false)
const authToken = ref<string>('')
const isRecentSoldPriceOpen = ref<boolean>(false)
const soldPriceSum = ref<SoldPriceSum[]>([])
const tableSum = ref<number>(0)

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

function triggerSearchItemLoading() {
  if (isSearchItem.value) return

  isSearchItem.value = true
}

function closeSearchItemLoading() {
  if (!isSearchItem.value) return

  setTimeout(() => {
    isSearchItem.value = false
  }, 300)
}

function initSoldItemInput() {
  soldItemInput.itemId = 0
  soldItemInput.itemIcon = 0
  soldItemInput.itemName = ''
  soldItemInput.quantity = 0
  soldItemInput.price = 0
  soldItemInput.soldDate = ''
  soldItemInput.url = fallbackImg
}

function triggerSuccessToast(msg: string) {
  toastMsg.value = msg
  toastType.value = 'success'
  showToast.value = true
}

function triggerErrorToast(msg: string) {
  toastMsg.value = msg
  toastType.value = 'error'
  showToast.value = true
}

function triggerHttpErrorToast(msg: string, status: number = 0) {
  toastMsg.value = `[${status}] ${msg}`
  toastType.value = 'error'
  showToast.value = true
}

const dateYearOptions = ref<number[]>([])
for (let i = now.year; i >= 2025; i--) {
  dateYearOptions.value.push(i)
}

async function fetchItemData() {
  const params = new URLSearchParams()
  params.append('name', soldItemInput.itemName)

  const url = apiUrl(`/api/submarine/item?${params.toString()}`)

  initSoldItemInput()
  triggerSearchItemLoading()

  try {
    const response = await axios.get<ItemDetail>(url)
    if (response.status === 200 && response.data.id > 0) {
      soldItemInput.itemId = response.data.id
      soldItemInput.itemName = response.data.name
      soldItemInput.itemIcon = response.data.icon
      soldItemInput.url = parseItemIconUrl(response.data.id, response.data.icon)
    }
  } catch (e) {
    triggerHttpErrorToast(messageFromError(e))
    console.error(e)
  } finally {
    closeSearchItemLoading()
  }
}

async function fetchSoldItems() {
  const url = apiUrl('/api/submarine/list')
  triggerLoading()
  
  if (!dataTable.value) {
    dataTable.value = new DataTable('#dataTable', options)
  }

  try {
    const start = DateTime.fromObject({ year: searchYear.value, month: searchMonth.value, day: 1 })
    const end = start.endOf('month')

    const reqBody: SearchReqBody = {
      startDate: start.toISODate()!,
      endDate: end.toISODate()!
    }

    const response = await axios.post(url, reqBody)

    let data: any[] = []
    let sum = 0

    if (response.status === 200) {
      const soldItems = plainToInstance<SoldItem, any[]>(SoldItem, response.data)
      for (const item of soldItems) {
        data.push(item.toDataTableData())
        sum += item.price
      }
      tableSum.value = sum
    } else {
      triggerHttpErrorToast('조회 오류', response.status)
    }

    dataTable.value.data.data = []
    dataTable.value.insert(data)
  } catch (e) {
    triggerHttpErrorToast(messageFromError(e))
    console.error(e)
  } finally {
    closeLoading()
  }
}

function openDeleteModal() {
  const checkboxes = document.querySelectorAll('.chk:checked')

  const ids: number[] = []

  checkboxes.forEach(checkbox => {
    const id = checkbox.getAttribute('data-id')!
    ids.push(Number(id))
  })

  if (ids.length === 0) {
    return triggerErrorToast('선택된 아이템이 없습니다.')
  }

  checkedIds.value = ids
  isDeleteModalOpen.value = true
}

function closeDeleteModal() {
  checkedIds.value = []
  isDeleteModalOpen.value = false
}

async function confirmDeleteModal() {
  if (!authToken.value) {
    return triggerErrorToast('인증토큰을 확인하세요')
  }

  const ids = checkedIds.value.join(',')
  const url = apiUrl(`/api/submarine/${ids}`)
  triggerLoading()

  try {
    const response = await axios.delete(url, { headers: { 'Authorization': authToken.value } })
    if (response.status === 200) {
      triggerSuccessToast('삭제 성공')
    } else {
      triggerHttpErrorToast('삭제 실패', response.status)
    }
  } catch (e) {
    triggerHttpErrorToast(messageFromError(e))
    console.error(e)
  } finally {
    closeLoading()
    closeDeleteModal()
  }
}

function openInputModal() {
  isInputModalOpen.value = true
}

function closeInputModal() {
  initSoldItemInput()

  isInputModalOpen.value = false
}

async function confirmInputModal() {
  const reqBody: SoldItemInputReqBody = {
    itemId: Number(soldItemInput.itemId),
    itemIcon: Number(soldItemInput.itemIcon),
    itemName: soldItemInput.itemName,
    quantity: Number(soldItemInput.quantity),
    price: Number(soldItemInput.price),
    soldDate: soldItemInput.soldDate,
  }

  if (!authToken.value) {
    return triggerErrorToast('인증토큰을 확인하세요')
  }

  if (reqBody.itemId < 0) {
    return triggerErrorToast('아이템을 검색하세요')
  }

  if (reqBody.quantity < 0) {
    return triggerErrorToast('수량을 확인하세요')
  }

  if (reqBody.price < 0) {
    return triggerErrorToast('판매가격을 확인하세요')
  }

  if (!reqBody.soldDate) {
    return triggerErrorToast('판매일을 확인하세요')
  }

  const url = apiUrl(`/api/submarine`)
  triggerLoading()

  try {
    const response = await axios.post(url, reqBody, { headers: { 'Authorization': authToken.value } })

    if (response.status === 200) {
      triggerSuccessToast('저장 성공')
    } else {
      triggerHttpErrorToast('저장 실패', response.status)
    }
  } catch (e) {
    triggerHttpErrorToast(messageFromError(e))
    console.error(e)
  } finally {
    closeLoading()
    closeInputModal()
  }
}

function openChatInputModal() {
  isChatInputModalOpen.value = true
}

function closeChatInputModal() {
  soldItemBulkInput.text = ''
  isChatInputModalOpen.value = false
}

async function confirmChatInputModal() {
  if (!authToken.value) {
    return triggerErrorToast('인증토큰을 확인하세요')
  }

  if (!soldItemBulkInput.text) {
    return triggerErrorToast('판매내역을 확인하세요')
  }

  const url = apiUrl(`/api/submarine/bulk`)
  triggerLoading()

  try {
    const response = await axios.post(url, {
      text: soldItemBulkInput.text,
    } as SoldItemBulkInput, { headers: { 'Authorization': authToken.value } })

    if (response.status === 200) {
      triggerSuccessToast('저장 성공')
    } else {
      triggerHttpErrorToast('저장 실패', response.status)
    }
  } catch (e) {
    triggerHttpErrorToast(messageFromError(e))
    console.error(e)
  } finally {
    closeLoading()
    closeChatInputModal()
  }
}

function defaultImage(e: Event) {
  const target = e.target as HTMLImageElement
  target.src = fallbackImg
  target.onerror = null
}

async function openRecentSoldPriceModal() {
  const url = apiUrl('/api/submarine/recentSoldPriceSum')

  triggerLoading()

  try {
    const response = await axios.get<SoldPriceSum[]>(url)
    if (response.status === 200) {
      soldPriceSum.value = response.data
      isRecentSoldPriceOpen.value = true
    } else {
      triggerHttpErrorToast('조회 오류', response.status)
    }
  } catch (e) {
    triggerHttpErrorToast(messageFromError(e))
    console.error(e)
  } finally {
    closeLoading()
  }
}

function closeRecentSoldPriceModal() {
  soldPriceSum.value = []
  isRecentSoldPriceOpen.value = false
}

onMounted(() => {
  fetchSoldItems()
})
</script>

<template>
  <Breadcrumb menu="FF14 고잉마샤호"/>

  <div class="mt-5"></div>

  <div class="bg-white dark:bg-gray-900">
    <form>
      <div class="grid items-end gap-6 mb-6 md:grid-cols-5">
        <div class="relative">
          <label class="block mb-2.5 text-sm font-medium  dark:text-white text-heading">년도</label>
          <select
            v-model="searchYear"
            class="block w-full px-2.5 py-2 bg-neutral-secondary-medium border border-default-medium text-heading text-xs rounded-base focus:ring-brand focus:border-brand shadow-xs placeholder:text-body">
            <option v-for="year in dateYearOptions" :value="year">{{ year }}</option>
          </select>
        </div>

        <div class="relative">
          <label class="block mb-2.5 text-sm font-medium dark:text-white text-heading">월</label>
          <input type="text" v-model="searchMonth"
                 class="bg-neutral-secondary-medium border border-default-medium text-heading text-xs rounded-base focus:ring-brand focus:border-brand block w-full px-2.5 py-2 shadow-xs placeholder:text-body"
                 required/>
        </div>

        <div class="relative">
          <button type="button"
                  @click="fetchSoldItems"
                  class="text-white bg-brand box-border border border-transparent hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium shadow-xs font-medium leading-5 rounded-base text-xs px-3 py-1.5 focus:outline-none cursor-pointer">
            <i class="fa fa-search"></i>
          </button>
        </div>
      </div>
    </form>

    <div class="col-span-1 space-x-1.5 flex">
      <button type="button" @click="openInputModal"
              class="inline-flex items-center justify-center text-white bg-brand hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium shadow-xs font-medium rounded-base text-xs px-3 py-2.5 focus:outline-none cursor-pointer">
        <i class="fa fa-pencil"></i>
      </button>

      <button type="button" @click="openChatInputModal"
              class="inline-flex items-center justify-center text-white bg-brand hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium shadow-xs font-medium rounded-base text-xs px-3 py-2.5 focus:outline-none cursor-pointer">
        <i class="fa fa-message"></i>
      </button>

      <button type="button" @click="openRecentSoldPriceModal"
              class="inline-flex items-center justify-center text-white bg-brand hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium shadow-xs font-medium rounded-base text-xs px-3 py-2.5 focus:outline-none cursor-pointer">
        <i class="fa fa-chart-area"></i>
      </button>

      <button type="button" @click="openDeleteModal"
              class="inline-flex items-center justify-center text-white bg-danger hover:bg-danger-strong focus:ring-4 focus:ring-danger-medium shadow-xs font-medium rounded-base text-xs px-3 py-2.5 focus:outline-none cursor-pointer">
        <i class="fa fa-trash"></i>
      </button>

      <button type="button"
              class="inline-flex items-center justify-center text-white bg-success hover:bg-success-strong focus:ring-4 focus:ring-success-medium shadow-xs font-medium rounded-base text-xs px-3 py-2.5 focus:outline-none cursor-pointer">
        <i class="fa-solid fa-sack-dollar"></i>
        <span class="pl-1">수입: {{ formattedPrice(tableSum) }}</span>
      </button>
    </div>

    <table id="dataTable" class="w-full text-sm text-left rtl:text-right text-body"></table>

    <div tabindex="-1" v-if="isDeleteModalOpen"
         class="overflow-y-auto overflow-x-hidden fixed inset-0 z-50 flex items-center justify-center w-full h-[calc(100%-1rem)] max-h-full bg-gray-900/70">
      <div class="relative w-full max-w-md max-h-full">
        <div class="bg-neutral-primary-soft border border-default rounded-base shadow-sm p-4 md:p-6">
          <button type="button"
                  @click="closeDeleteModal"
                  class="absolute top-3 inset-e-2.5 text-body bg-transparent hover:bg-neutral-tertiary hover:text-heading rounded-base text-sm w-9 h-9 ms-auto inline-flex justify-center items-center"
                  data-modal-hide="popup-modal">
            <i class="fa-solid fa-xmark"></i>
            <span class="sr-only">닫기</span>
          </button>
          <div class="p-4 md:p-5 text-center">
            <svg class="mx-auto mb-4 text-fg-disabled w-12 h-12" aria-hidden="true" xmlns="http://www.w3.org/2000/svg"
                 width="24" height="24" fill="none" viewBox="0 0 24 24">
              <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 13V8m0 8h.01M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z"/>
            </svg>
            <h3 class="mb-6 text-body">선택한 항목을 삭제 하시겠습니까?</h3>
            <div class="flex items-center space-x-4 justify-center">
              <button @click="closeDeleteModal" data-modal-hide="popup-modal" type="button"
                      class="text-body bg-neutral-secondary-medium box-border border border-default-medium hover:bg-neutral-tertiary-medium hover:text-heading focus:ring-4 focus:ring-neutral-tertiary shadow-xs font-medium leading-5 rounded-base text-sm px-4 py-2.5 focus:outline-none">
                취소
              </button>
              <button @click="confirmDeleteModal" data-modal-hide="popup-modal" type="button"
                      class="text-white bg-danger box-border border border-transparent hover:bg-danger-strong focus:ring-4 focus:ring-danger-medium shadow-xs font-medium leading-5 rounded-base text-sm px-4 py-2.5 focus:outline-none">
                삭제
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div tabindex="-1" v-if="isInputModalOpen"
         class="overflow-y-auto overflow-x-hidden fixed inset-0 z-50 flex items-center justify-center w-full h-[calc(100%-1rem)] max-h-full bg-gray-900/70">
      <div class="relative w-full max-w-md max-h-full">
        <div class="relative bg-neutral-primary-soft border border-default rounded-base shadow-sm p-4 md:p-6">
          <div class="flex items-center justify-between border-b border-default pb-4 md:pb-5">
            <h3 class="text-lg font-medium text-heading">
              판매내역
            </h3>
            <button type="button" @click="closeInputModal"
                    class="text-body bg-transparent hover:bg-neutral-tertiary hover:text-heading rounded-base text-sm w-9 h-9 ms-auto inline-flex justify-center items-center"
                    data-modal-hide="authentication-modal">
              <i class="fa-solid fa-xmark"></i>
              <span class="sr-only">닫기</span>
            </button>
          </div>
          <form class="pt-4 md:pt-6">
            <div class="mb-4">
              <label class="block mb-2.5 text-xs font-medium text-heading">인증토큰</label>
              <input type="text" v-model="authToken"
                     class="bg-neutral-secondary-medium border border-default-medium text-heading text-xs rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
                     placeholder="" required/>
            </div>
            <div class="mb-4">
              <label for="" class="block mb-2.5 text-xs font-medium text-heading">아이템명</label>
              <div class="relative">
                <input type="text" v-model="soldItemInput.itemName" @keyup.enter="fetchItemData"
                       class="bg-neutral-secondary-medium border border-default-medium text-heading text-xs rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
                       placeholder="" required/>

                <div v-if="isSearchItem"
                     class="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none -mt-14">
                  <svg class="w-5 h-5 text-blue-600 animate-spin" xmlns="http://www.w3.org/2000/svg" fill="none"
                       viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor"
                          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                </div>

                <div class="mt-2 flex items-center">
                  <img :src="soldItemInput.url" @error="defaultImage" alt="아이템" width="48" height="48"
                       class="dark:bg-white rounded-sm"/>
                  <span class="text-xs font-medium text-heading">[#{{ soldItemInput.itemId }}]</span>
                </div>
              </div>
            </div>
            <div class="mb-4">
              <label class="block mb-2.5 text-xs font-medium text-heading">수량</label>
              <input type="text" v-model="soldItemInput.quantity" v-number-only
                     class="bg-neutral-secondary-medium border border-default-medium text-heading text-xs rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
                     placeholder="" required/>
            </div>
            <div class="mb-4">
              <label class="block mb-2.5 text-xs font-medium text-heading">판매가격</label>
              <input type="text" v-model="soldItemInput.price" v-number-only
                     class="bg-neutral-secondary-medium border border-default-medium text-heading text-xs rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
                     placeholder="" required/>
            </div>
            <div class="mb-4">
              <label class="block mb-2.5 text-xs font-medium text-heading">판매일 (예시: {{ now.toISODate() }})</label>
              <input type="text" v-model="soldItemInput.soldDate" v-date-format
                     maxlength="10"
                     class="bg-neutral-secondary-medium border border-default-medium text-heading text-xs rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
                     placeholder="" required/>
            </div>
            <div class="flex items-center space-x-4 justify-center pt-4">
              <button @click="closeInputModal" data-modal-hide="popup-modal" type="button"
                      class="text-body bg-neutral-secondary-medium box-border border border-default-medium hover:bg-neutral-tertiary-medium hover:text-heading focus:ring-4 focus:ring-neutral-tertiary shadow-xs font-medium leading-5 rounded-base text-sm px-4 py-2.5 focus:outline-none">
                취소
              </button>
              <button @click="confirmInputModal" data-modal-hide="popup-modal" type="button"
                      class="text-white bg-brand box-border border border-transparent hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium shadow-xs font-medium leading-5 rounded-base text-sm px-4 py-2.5 focus:outline-none">
                확인
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <div tabindex="-1" v-if="isChatInputModalOpen"
         class="overflow-y-auto overflow-x-hidden fixed inset-0 z-50 flex items-center justify-center w-full h-[calc(100%-1rem)] max-h-full bg-gray-900/70">
      <div class="relative w-full max-w-md max-h-full">
        <div class="relative bg-neutral-primary-soft border border-default rounded-base shadow-sm p-4 md:p-6">
          <div class="flex items-center justify-between border-b border-default pb-4 md:pb-5">
            <h3 class="text-lg font-medium text-heading">
              판매내역 채팅메시지
            </h3>
            <button type="button" @click="closeChatInputModal"
                    class="text-body bg-transparent hover:bg-neutral-tertiary hover:text-heading rounded-base text-sm w-9 h-9 ms-auto inline-flex justify-center items-center"
                    data-modal-hide="authentication-modal">
              <i class="fa-solid fa-xmark"></i>
              <span class="sr-only">닫기</span>
            </button>
          </div>
          <form class="pt-4 md:pt-6">
            <div class="mb-4">
              <label class="block mb-2.5 text-xs font-medium text-heading">인증토큰</label>
              <input type="text" v-model="authToken"
                     class="bg-neutral-secondary-medium border border-default-medium text-heading text-xs rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
                     placeholder="" required/>
            </div>
            <div class="mb-4">
              <label class="block mb-2.5 text-xs font-medium text-heading">판매내역</label>
              <textarea v-model="soldItemBulkInput.text" rows="10"
                        class="block bg-neutral-secondary-medium border border-default-medium text-heading text-xs rounded-base focus:ring-brand focus:border-brand w-full p-3.5 shadow-xs placeholder:text-body"
                        placeholder="판매내역 채팅메시지" required></textarea>
            </div>
            <div class="flex items-center space-x-4 justify-center pt-4">
              <button @click="closeChatInputModal" data-modal-hide="popup-modal" type="button"
                      class="text-body bg-neutral-secondary-medium box-border border border-default-medium hover:bg-neutral-tertiary-medium hover:text-heading focus:ring-4 focus:ring-neutral-tertiary shadow-xs font-medium leading-5 rounded-base text-sm px-4 py-2.5 focus:outline-none">
                취소
              </button>
              <button @click="confirmChatInputModal" data-modal-hide="popup-modal" type="button"
                      class="text-white bg-brand box-border border border-transparent hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium shadow-xs font-medium leading-5 rounded-base text-sm px-4 py-2.5 focus:outline-none">
                확인
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- 최근 총수입 -->
    <div v-if="isRecentSoldPriceOpen" tabindex="-1"
         class="overflow-y-auto overflow-x-hidden fixed inset-0 z-50 flex justify-center items-center w-full md:inset-0 h-[calc(100%-1rem)] max-h-full bg-gray-900/70">
      <div class="relative p-4 w-full max-w-2xl max-h-full">
        <div class="relative bg-neutral-primary-soft border border-default rounded-base shadow-sm p-4 md:p-6">
          <div class="flex items-center justify-between border-b border-default pb-4 md:pb-5">
            <h3 class="text-lg font-medium text-heading">
              최근 총수입
            </h3>
            <button type="button" @click="closeRecentSoldPriceModal"
                    class="text-body bg-transparent hover:bg-neutral-tertiary hover:text-heading rounded-base text-sm w-9 h-9 ms-auto inline-flex justify-center items-center">
              <i class="fa-solid fa-xmark"></i>
              <span class="sr-only">닫기</span>
            </button>
          </div>
          <div class="space-y-4 md:space-y-6 py-4 md:py-2">
            <ul role="list" class="space-y-3 p-6 divide-y divide-default">
              <li class="flex items-center justify-between p-1" v-for="price in soldPriceSum">
                <div class="flex items-center text-body">
                  <i class="fa-solid fa-sack-dollar"></i>
                  <span class="pl-3">{{ price.yearMonth }}</span>
                </div>
                <span class="text-body font-medium">{{ formattedPrice(price.priceSum) }}</span>
              </li>
            </ul>
          </div>
          <div class="flex items-center justify-center border-t border-default space-x-4 pt-4 md:pt-5">
            <button type="button" @click="closeRecentSoldPriceModal"
                    class="text-body bg-neutral-secondary-medium box-border border border-default-medium hover:bg-neutral-tertiary-medium hover:text-heading focus:ring-4 focus:ring-neutral-tertiary shadow-xs font-medium leading-5 rounded-base text-sm px-4 py-2.5 focus:outline-none">
              닫기
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Toast -->
    <div class="fixed top-5 right-5 z-50 flex flex-col space-y-2">
      <Transition name="slide-fade">
        <Toast
          v-if="showToast"
          :message="toastMsg"
          :type="toastType"
          @close="showToast = false"
        />
      </Transition>
    </div>

    <!-- Loading -->
    <Teleport to="body">
      <LoadingOverlay :isLoading="isLoading"/>
    </Teleport>
  </div>
</template>

<!--suppress CssUnusedSymbol -->
<style scoped>
.slide-fade-enter-active {
  transition: all 0.3s ease-out;
}

.slide-fade-leave-active {
  transition: all 0.3s cubic-bezier(1, 0.5, 0.8, 1);
}

.slide-fade-enter-from,
.slide-fade-leave-to {
  transform: translateX(20px);
  opacity: 0;
}
</style>
