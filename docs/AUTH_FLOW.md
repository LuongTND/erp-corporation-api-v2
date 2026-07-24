# Auth Flow — ERP Corporation API v2

> Dành cho Frontend team. Cập nhật theo codebase ERP Corporation API v2.

---

## Endpoints

| Method | Route | Auth | Mô tả |
|---|---|---|---|
| POST | `/api/auth/login` | ❌ | Đăng nhập |
| POST | `/api/auth/refresh` | ❌ | Làm mới access token |
| POST | `/api/auth/logout` | ✅ | Đăng xuất |
| GET | `/api/auth/me` | ✅ | Thông tin user hiện tại |
| PUT | `/api/auth/me` | ✅ | Cập nhật profile |
| PUT | `/api/auth/change-password` | ✅ | Đổi mật khẩu |

---

## 1. Đăng nhập

```
POST /api/auth/login
```

**Request:**
```json
{
  "email": "user@company.com",
  "password": "!Abc1234"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "data": {
    "accessToken": "eyJ...",
    "refreshToken": "eyJ...",
    "expiresIn": 900
  }
}
```

**Lỗi:**
```json
{
  "isSuccess": false,
  "statusCode": 401,
  "message": "Email hoặc mật khẩu không đúng."
}
```

FE lưu `accessToken` vào memory, `refreshToken` vào httpOnly cookie hoặc localStorage.

---

## 2. Làm mới Access Token

```
POST /api/auth/refresh
```

**Request:**
```json
{
  "refreshToken": "eyJ..."
}
```

**Response:** cặp token mới (cả `accessToken` lẫn `refreshToken` đều cấp lại).

> Mỗi lần refresh, `refreshToken` cũ bị vô hiệu. FE phải lưu token mới ngay.

**Lỗi 401** → FE xoá token, đưa user về trang đăng nhập.

---

## 3. Đăng xuất

```
POST /api/auth/logout
Authorization: Bearer <accessToken>
```

Xoá refresh token phía server. FE xoá cả hai token local.

---

## 4. Thông tin người dùng hiện tại

```
GET /api/auth/me
Authorization: Bearer <accessToken>
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "userId": "...",
    "fullName": "Nguyễn Văn A",
    "email": "user@company.com",
    "roles": ["Admin"]
  }
}
```

---

## 5. Cập nhật Profile

```
PUT /api/auth/me
Authorization: Bearer <accessToken>
```

**Request:**
```json
{
  "fullName": "Nguyễn Văn B",
  "email": "new@company.com"
}
```

Trả về profile đã cập nhật (cùng format với `/me`).

---

## 6. Đổi mật khẩu

```
PUT /api/auth/change-password
Authorization: Bearer <accessToken>
```

**Request:**
```json
{
  "currentPassword": "!Abc1234",
  "newPassword": "!Xyz5678"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "statusCode": 200
}
```

---

## Hướng dẫn FE lưu token

```
accessToken  → memory (JS variable / Zustand store) — KHÔNG lưu localStorage
refreshToken → httpOnly cookie (khuyến nghị) hoặc localStorage nếu chưa có cookie support
```

Khi gọi API:
```
Authorization: Bearer <accessToken>
```

Khi nhận 401 → thử refresh → nếu refresh cũng 401 → đưa về login.
