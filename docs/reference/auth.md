# Auth Endpoints
> FE: `accessToken` → memory · `refreshToken` → httpOnly cookie

## Endpoints

| Method | Route | Auth | Mô tả |
|---|---|---|---|
| POST | `/api/auth/login` | ❌ | Đăng nhập → trả accessToken + refreshToken |
| POST | `/api/auth/refresh` | ❌ | Làm mới token — cả 2 token cấp lại, token cũ vô hiệu |
| POST | `/api/auth/logout` | ✅ | Xóa refreshToken server-side |
| GET | `/api/auth/me` | ✅ | Profile + roles của user hiện tại |
| PUT | `/api/auth/me` | ✅ | Cập nhật fullName, email |
| PUT | `/api/auth/change-password` | ✅ | Đổi mật khẩu |

## Request/Response pattern

```json
// POST /api/auth/login
{ "email": "user@co.com", "password": "!Abc1234" }
→ { "data": { "accessToken": "eyJ...", "refreshToken": "eyJ...", "expiresIn": 900 } }

// POST /api/auth/refresh
{ "refreshToken": "eyJ..." }
→ cặp token mới

// GET /api/auth/me
→ { "data": { "userId": "...", "fullName": "...", "email": "...", "roles": ["Admin"] } }
```

## FE flow

```
401 → POST /refresh → 401 lại → redirect login
Authorization: Bearer <accessToken>
```
