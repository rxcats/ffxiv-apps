<script setup lang="ts">
import { ref } from 'vue'
import Breadcrumb from '@/components/Breadcrumb.vue'

const waitSecondsPattern: RegExp = /<wait\.(\d+)>/

const macroText1 = ref<string>('')
const macroText2 = ref<string>('')
const macroSeconds1 = ref<number>(0)
const macroSeconds2 = ref<number>(0)
const isCalculating = ref<boolean>(false)

function parseSeconds(text: string): number {
  let seconds: number = 0

  const lines = text.split("\n")

  for (let line of lines) {
    const match: RegExpMatchArray | null = line.trim().match(waitSecondsPattern)

    if (!match || !match[1]) {
      continue
    }

    seconds += parseInt(match[1], 10)
  }

  return seconds
}

function startLoading() {
  if (isCalculating.value) return

  isCalculating.value = true
}

function stopLoading() {
  if (!isCalculating.value) return
  
  setTimeout(() => {
    isCalculating.value = false
  }, 300)
}

function onCalculating() {
  startLoading()
  macroSeconds1.value = parseSeconds(macroText1.value)
  macroSeconds2.value = parseSeconds(macroText2.value)
  stopLoading()
}
</script>

<template>
  <Breadcrumb menu="FF14 매크로"/>

  <div class="mt-5"></div>

  <button type="button" @click="onCalculating"
          class="text-white bg-brand box-border border border-transparent hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium shadow-xs font-medium leading-5 rounded-base text-sm px-4 py-2.5 focus:outline-none">
    <i :style="{ display: isCalculating ? 'none' : '' }" class="fa fa-calculator"></i>
    <i :style="{ display: !isCalculating ? 'none' : '' }" class="fa-solid fa-spinner animate-spin"></i>
    계산
  </button>

  <div class="mt-3"></div>

  <span
    class="mb-2 inline-flex items-center bg-success-soft border border-success-subtle text-fg-success-strong text-sm font-medium leading-none px-2 py-1 rounded">
        <i class="fa fa-clock mr-1"></i> {{ macroSeconds1 }} 초
      </span>
  <textarea v-model="macroText1" rows="10"
            class="bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full p-3.5 shadow-xs placeholder:text-body"
            placeholder="매크로1 입력"></textarea>

  <div class="mt-3"></div>

  <span
    class="mb-2 inline-flex items-center bg-success-soft border border-success-subtle text-fg-success-strong text-sm font-medium leading-none px-2 py-1 rounded">
        <i class="fa fa-clock mr-1"></i> {{ macroSeconds2 }} 초
      </span>
  <textarea v-model="macroText2" rows="10"
            class="bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full p-3.5 shadow-xs placeholder:text-body"
            placeholder="매크로2 입력"></textarea>

</template>

<style scoped>

</style>
