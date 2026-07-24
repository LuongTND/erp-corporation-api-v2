# Auth Flow — SSWMS API

> Cập nhật theo codebase hiện tại. Dành cho Frontend team.

---

## 1. Đăng ký Tenant

```
POST /api/auth/register
```

**Request body:**
```json
{
  "tenantName": "Công ty ABC",
  "ownerName": "Trần Văn A",
  "phone": "0901234567",       // tuỳ chọn
  "email": "owner@abc.com",
  "address": "123 Nguyễn Huệ, TP.HCM",
  "password": "!Abc1234",
  "confirmPassword": "!Abc1234",
  "acceptTerms": true
}
```

**Password rules:** ≥8 ký tự, có chữ hoa, chữ thường, số, ký tự đặc biệt.

**Response:**
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success",
  "data": "Đăng ký thành công. Vui lòng kiểm tra email để xác minh tài khoản."
}
```

Sau khi đăng ký, hệ thống tự gửi email chứa link xác minh (TTL 15 phút). FE chuyển sang trang đăng nhập, hiển thị thông báo yêu cầu xác minh email.

---

## 2. Xác minh Email

User bấm link trong email → trình duyệt gọi thẳng API:

```
GET /api/auth/verify-email?token=<token>
```

**Response thành công:**
```json
{
  "isSuccess": true,
  "data": "Xác minh email thành công."
}
```

**Response nếu link hết hạn / dùng lại:**
```json
{
  "isSuccess": false,
  "statusCode": 400,
  "message": "Link xác minh đã hết hạn hoặc không hợp lệ."
}
```

FE có thể hiển thị trang kết quả xác minh dựa trên response này (không cần redirect thêm từ BE).

---

## 3. Đăng nhập

```
POST /api/auth/login
```

**Request body:**
```json
{
  "email": "owner@abc.com",
  "password": "!Abc1234"
}
```

### Trường hợp 1 — Không có 2FA

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "accessToken": "eyJ...",
    "refreshToken": "eyJ...",
    "expiresIn": 1209600,
    "requires2FA": false,
    "tempToken": null
  }
}
```

FE lưu `accessToken` và `refreshToken`, chuyển vào app.

### Trường hợp 2 — Có bật 2FA

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "accessToken": null,
    "refreshToken": null,
    "requires2FA": true,
    "tempToken": "eyJ..."
  }
}
```

FE hiển thị màn hình nhập OTP từ Google/Microsoft Authenticator, giữ `tempToken` để dùng ở bước tiếp theo.

### Lỗi đăng nhập & Captcha

| Lần sai | Response |
|---|---|
| Lần 1 | `"Email hoặc mật khẩu không đúng."` |
| Lần 2 | Message + `errors.requiresCaptcha = ["true"]` → **FE hiển thị captcha** |
| Lần 3 | Tài khoản bị khoá. Email thông báo gửi cho Tenant Owner (nếu là staff). |

```json
// Ví dụ response lần sai thứ 2
{
  "isSuccess": false,
  "statusCode": 401,
  "message": "Nhập sai mật khẩu, còn 1 lần nữa sẽ bị khoá tài khoản.",
  "errors": {
    "requiresCaptcha": ["true"]
  }
}
```

---

## 4. Xác minh 2FA

```
POST /api/auth/verify-2fa
```

**Request body:**
```json
{
  "tempToken": "eyJ...",   // từ bước login
  "otp": "481239"          // 6 số từ Google/Microsoft Authenticator
}
```

**Response:** giống Trường hợp 1 đăng nhập thành công (trả về đầy đủ `accessToken` + `refreshToken`).

OTP hợp lệ trong 30 giây (TOTP standard) + dung sai 1 window (±30s).

---

## 5. Làm mới Access Token

```
POST /api/auth/refresh
```

**Request body:**
```json
{
  "refreshToken": "eyJ..."
}
```

**Response:** trả về cặp token mới (cả `accessToken` lẫn `refreshToken` đều được cấp lại).

> **Lưu ý:** Mỗi lần refresh, `refreshToken` cũ bị vô hiệu và thay bằng token mới. FE phải lưu lại token mới ngay.

**Lỗi:**
```json
{
  "isSuccess": false,
  "statusCode": 401,
  "message": "Refresh token is invalid or expired."
}
```

→ FE xoá token, đưa user về màn hình đăng nhập.

---

## 6. Đăng xuất

```
POST /api/auth/logout
Authorization: Bearer <accessToken>
```

Xoá refresh token khỏi Redis. FE xoá cả hai token.

---

## 7. Cài đặt 2FA (trong Settings)

### Bật 2FA

**Bước 1 — Lấy QR code:**
```
POST /api/settings/2fa/setup
Authorization: Bearer <accessToken>
```

Response:
```json
{
  "data": {
    "secret": "JBSWY3DPEHPK3PXP",
    "qrUri": "otpauth://totp/SSWMS:owner@abc.com?secret=JBSWY3DPEHPK3PXP&issuer=SSWMS"
  }
}
```

FE render QR code từ `qrUri` (dùng thư viện như `qrcode.react`). User quét bằng Google/Microsoft Authenticator.

**Bước 2 — Xác nhận:**
```
POST /api/settings/2fa/confirm
Authorization: Bearer <accessToken>
```

Body:
```json
{ "otp": "481239" }
```

Response: `"Bật xác thực hai yếu tố thành công."`

### Tắt 2FA

```
DELETE /api/settings/2fa
Authorization: Bearer <accessToken>
```

Response: `"Đã tắt xác thực hai yếu tố."`

---

## 8. Quên / Đặt lại mật khẩu

**Quên mật khẩu:**
```
POST /api/auth/forgot-password
Body: { "email": "owner@abc.com" }
```

Gửi email chứa reset link (TTL 15 phút). Luôn trả thành công dù email không tồn tại (tránh email enumeration).

**Đặt lại mật khẩu:**
```
POST /api/auth/reset-password
Body: { "token": "...", "newPassword": "!Abc1234", "confirmPassword": "!Abc1234" }
```

**Đổi mật khẩu (đã đăng nhập):**
```
PUT /api/auth/change-password   [Bearer token]
Body: { "currentPassword": "...", "newPassword": "...", "confirmPassword": "..." }
```

---

## 9. Thông tin người dùng hiện tại

```
GET /api/auth/me
Authorization: Bearer <accessToken>
```

---

## 10. Tổng hợp Endpoints

| Method | Route | Auth | Mô tả |
|---|---|---|---|
| POST | `/api/auth/register` | ❌ | Đăng ký tenant + owner |
| GET | `/api/auth/verify-email?token=` | ❌ | Xác minh email từ link |
| POST | `/api/auth/login` | ❌ | Đăng nhập bằng email |
| POST | `/api/auth/verify-2fa` | ❌ | Xác minh TOTP 2FA |
| POST | `/api/auth/refresh` | ❌ | Làm mới access token |
| POST | `/api/auth/logout` | ✅ | Đăng xuất |
| POST | `/api/auth/forgot-password` | ❌ | Gửi email reset mật khẩu |
| POST | `/api/auth/reset-password` | ❌ | Đặt lại mật khẩu |
| PUT | `/api/auth/change-password` | ✅ | Đổi mật khẩu |
| GET | `/api/auth/me` | ✅ | Thông tin người dùng hiện tại |
| POST | `/api/settings/2fa/setup` | ✅ | Lấy QR code bật 2FA |
| POST | `/api/settings/2fa/confirm` | ✅ | Xác nhận bật 2FA |
| DELETE | `/api/settings/2fa` | ✅ | Tắt 2FA |

---

## 11. Hướng dẫn FE lưu token

```
accessToken  → memory (biến JS / Zustand store) — KHÔNG lưu localStorage
refreshToken → httpOnly cookie (khuyến nghị) hoặc localStorage nếu chưa có cookie support
```

Khi gọi API:
```
Authorization: Bearer <accessToken>
```

Khi nhận 401 → thử refresh → nếu refresh cũng 401 → đưa về login.
