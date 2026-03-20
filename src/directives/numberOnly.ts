import type { Directive } from 'vue';

export const numberOnly: Directive = {
  mounted(el: HTMLInputElement) {
    const inputEl = el.tagName === 'INPUT' ? el : el.querySelector('input')

    if (!inputEl) return

    inputEl.addEventListener('input', (e: Event) => {
      const target = e.target as HTMLInputElement
      const prevValue = target.value

      // 숫자가 아닌 문자 제거
      const cleanValue = prevValue.replace(/\D/g, '')

      // 값이 변경되었을 때만 처리 (무한 루프 방지)
      if (prevValue !== cleanValue) {
        target.value = cleanValue

        // 중요: v-model이 업데이트되도록 input 이벤트를 수동으로 발생시킴
        target.dispatchEvent(new Event('input'))
      }
    })
  }
}
