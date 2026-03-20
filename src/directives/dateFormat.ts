import type { Directive } from 'vue'

export const dateFormat: Directive = {
  mounted(el: HTMLInputElement) {
    const inputEl = el.tagName === 'INPUT' ? el : el.querySelector('input')
    if (!inputEl) return;

    inputEl.addEventListener('input', (e: Event) => {
      const target = e.target as HTMLInputElement;
      let val = target.value.replace(/\D/g, '')

      if (val.length > 8) val = val.substring(0, 8)

      let formatted = ''
      if (val.length <= 4) {
        formatted = val
      } else if (val.length <= 6) {
        formatted = `${val.substring(0, 4)}-${val.substring(4)}`
      } else {
        formatted = `${val.substring(0, 4)}-${val.substring(4, 6)}-${val.substring(6)}`
      }

      if (target.value !== formatted) {
        target.value = formatted
        target.dispatchEvent(new Event('input'))
      }
    })

    inputEl.addEventListener('keydown', (e: KeyboardEvent) => {
      if (e.key === 'Backend') {
        // 마지막 글자가 '-'인 경우 숫자를 하나 더 지우고 싶다면 여기서 처리
      }
    })
  }
}
