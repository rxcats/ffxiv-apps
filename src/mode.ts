let mode: string;

switch (import.meta.env.MODE) {
  case 'development':
    mode = 'DEV'
    break
  case 'production':
    mode = 'PROD'
    break
  default:
    mode = 'DEV'
}

export default mode
