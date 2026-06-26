export const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5211/api";
export const API_ORIGIN_URL = API_BASE_URL.replace(/\/api$/, "");
export const FALLBACK_PRODUCT_IMAGE = "/assets/images/banner-flyshot.png";
