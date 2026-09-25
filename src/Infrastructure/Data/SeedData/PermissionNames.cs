namespace Infrastructure;

public static class PermissionNames
{
    public static readonly Dictionary<string, (string Name, string Description)> Map = new()
    {
        // ── Nhật ký thao tác ──────────────────────────────────────────────
        ["rbac:audit-logs:view-list"] = ("Xem nhật ký thao tác", "Xem danh sách toàn bộ nhật ký hoạt động hệ thống"),

        // ── Quyền hệ thống ────────────────────────────────────────────────
        ["rbac:permissions:view-list"] = ("Xem danh sách quyền", "Xem toàn bộ quyền hiện có trong hệ thống"),
        ["rbac:permissions:delete"]    = ("Xóa quyền", "Xóa quyền khỏi hệ thống"),

        // ── Vai trò ───────────────────────────────────────────────────────
        ["rbac:roles:view-list"]         = ("Xem danh sách vai trò", "Xem danh sách tất cả vai trò"),
        ["rbac:roles:view-users"]        = ("Xem người dùng trong vai trò", "Xem danh sách nhân sự thuộc một vai trò"),
        ["rbac:roles:create"]            = ("Tạo vai trò", "Tạo mới vai trò trong hệ thống"),
        ["rbac:roles:update"]            = ("Cập nhật vai trò", "Chỉnh sửa thông tin vai trò"),
        ["rbac:roles:delete"]            = ("Xóa vai trò", "Xóa vai trò khỏi hệ thống"),
        ["rbac:roles:assign-permission"] = ("Gán quyền cho vai trò", "Thêm hoặc thu hồi quyền của một vai trò"),
        ["rbac:roles:sync-users"]        = ("Đồng bộ người dùng vào vai trò", "Cập nhật lại danh sách nhân sự thuộc vai trò"),

        // ── Nhân sự (Người dùng) ─────────────────────────────────────────
        ["hrm:users:create"]               = ("Tạo nhân sự", "Tạo hồ sơ nhân sự mới"),
        ["hrm:users:view"]                 = ("Xem hồ sơ nhân sự", "Xem thông tin chi tiết hồ sơ nhân sự"),
        ["hrm:users:view-history"]         = ("Xem lịch sử nhân sự", "Xem lịch sử thay đổi của hồ sơ nhân sự"),
        ["hrm:users:export"]               = ("Xuất dữ liệu nhân sự", "Xuất danh sách nhân sự ra file"),
        ["hrm:users:update-profile"]       = ("Cập nhật hồ sơ nhân sự", "Chỉnh sửa thông tin cá nhân của nhân sự"),
        ["hrm:users:update-custom-fields"] = ("Cập nhật trường tùy chỉnh", "Chỉnh sửa các trường mở rộng trong hồ sơ nhân sự"),
        ["hrm:users:update-status"]        = ("Cập nhật trạng thái nhân sự", "Thay đổi trạng thái làm việc của nhân sự"),
        ["hrm:users:lock"]                 = ("Khóa tài khoản nhân sự", "Khóa hoặc mở khóa tài khoản nhân sự"),
        ["hrm:users:assign-employee-type"] = ("Gán loại nhân sự", "Gán loại hợp đồng / phân loại nhân sự"),
        ["hrm:users:remove-job-level"]     = ("Xóa cấp bậc nhân sự", "Gỡ bỏ cấp bậc công việc của nhân sự"),
        ["hrm:users:add-department"]       = ("Thêm nhân sự vào phòng ban", "Gán nhân sự vào một phòng ban"),
        ["hrm:users:update-department"]    = ("Cập nhật phòng ban nhân sự", "Thay đổi thông tin phòng ban của nhân sự"),
        ["hrm:users:remove-department"]    = ("Xóa nhân sự khỏi phòng ban", "Gỡ nhân sự ra khỏi phòng ban"),
        ["hrm:users:transfer-department"]  = ("Chuyển phòng ban nhân sự", "Chuyển nhân sự sang phòng ban khác"),
        ["hrm:users:assign-role"]          = ("Gán vai trò cho nhân sự", "Phân quyền vai trò cho nhân sự"),
        ["hrm:users:revoke-role"]          = ("Thu hồi vai trò nhân sự", "Gỡ bỏ vai trò đã gán cho nhân sự"),
        ["hrm:users:set-scope"]            = ("Thiết lập phạm vi truy cập", "Xác định phạm vi dữ liệu nhân sự được phép thao tác"),

        // ── Phòng ban ─────────────────────────────────────────────────────
        ["hrm:departments:view-list"]    = ("Xem danh sách phòng ban", "Xem toàn bộ danh sách phòng ban"),
        ["hrm:departments:view-detail"]  = ("Xem chi tiết phòng ban", "Xem thông tin chi tiết một phòng ban"),
        ["hrm:departments:view-tree"]    = ("Xem cây phòng ban", "Xem sơ đồ phân cấp phòng ban"),
        ["hrm:departments:view-members"] = ("Xem thành viên phòng ban", "Xem danh sách nhân sự trong phòng ban"),
        ["hrm:departments:create"]       = ("Tạo phòng ban", "Tạo mới phòng ban"),
        ["hrm:departments:update"]       = ("Cập nhật phòng ban", "Chỉnh sửa thông tin phòng ban"),
        ["hrm:departments:delete"]       = ("Xóa phòng ban", "Xóa phòng ban khỏi hệ thống"),

        // ── Cấp bậc phòng ban ─────────────────────────────────────────────
        ["hrm:department-job-levels:view-list"]           = ("Xem cấp bậc phòng ban", "Xem danh sách cấp bậc trong phòng ban"),
        ["hrm:department-job-levels:view-detail"]         = ("Xem chi tiết cấp bậc phòng ban", "Xem thông tin chi tiết cấp bậc"),
        ["hrm:department-job-levels:create"]              = ("Tạo cấp bậc phòng ban", "Tạo mới cấp bậc cho phòng ban"),
        ["hrm:department-job-levels:update"]              = ("Cập nhật cấp bậc phòng ban", "Chỉnh sửa thông tin cấp bậc"),
        ["hrm:department-job-levels:delete"]              = ("Xóa cấp bậc phòng ban", "Xóa cấp bậc khỏi phòng ban"),
        ["hrm:department-job-levels:assign-kpi-template"] = ("Gán mẫu KPI cho cấp bậc", "Liên kết mẫu KPI với cấp bậc trong phòng ban"),

        // ── Cấp bậc công việc ─────────────────────────────────────────────
        ["hrm:job-levels:view-list"]   = ("Xem danh sách cấp bậc", "Xem toàn bộ cấp bậc công việc"),
        ["hrm:job-levels:view-detail"] = ("Xem chi tiết cấp bậc", "Xem thông tin chi tiết một cấp bậc"),
        ["hrm:job-levels:create"]      = ("Tạo cấp bậc", "Tạo mới cấp bậc công việc"),
        ["hrm:job-levels:update"]      = ("Cập nhật cấp bậc", "Chỉnh sửa thông tin cấp bậc"),
        ["hrm:job-levels:delete"]      = ("Xóa cấp bậc", "Xóa cấp bậc công việc"),

        // ── Loại nhân sự ─────────────────────────────────────────────────
        ["hrm:employee-types:view-list"] = ("Xem loại nhân sự", "Xem danh sách các loại hợp đồng / phân loại nhân sự"),
        ["hrm:employee-types:create"]    = ("Tạo loại nhân sự", "Tạo mới loại nhân sự"),
        ["hrm:employee-types:update"]    = ("Cập nhật loại nhân sự", "Chỉnh sửa loại nhân sự"),
        ["hrm:employee-types:delete"]    = ("Xóa loại nhân sự", "Xóa loại nhân sự"),

        // ── Cửa hàng ─────────────────────────────────────────────────────
        ["hrm:stores:view-list"]       = ("Xem danh sách cửa hàng", "Xem toàn bộ danh sách cửa hàng"),
        ["hrm:stores:delete"]          = ("Xóa cửa hàng", "Xóa cửa hàng khỏi hệ thống"),
        ["hrm:stores:sync"]            = ("Đồng bộ cửa hàng", "Đồng bộ dữ liệu cửa hàng từ hệ thống ngoài"),
        ["hrm:stores:view-hours"]      = ("Xem giờ làm việc cửa hàng", "Xem lịch giờ hoạt động của cửa hàng"),
        ["hrm:stores:toggle-active"]   = ("Bật/tắt cửa hàng", "Kích hoạt hoặc vô hiệu hóa cửa hàng"),
        ["hrm:stores:update-hours"]    = ("Cập nhật giờ làm việc cửa hàng", "Chỉnh sửa lịch giờ hoạt động cửa hàng"),
        ["hrm:stores:assign-manager"]  = ("Gán quản lý cửa hàng", "Chỉ định quản lý cho cửa hàng"),
        ["hrm:stores:view-members"]    = ("Xem nhân viên cửa hàng", "Xem danh sách nhân viên trong cửa hàng"),
        ["hrm:stores:add-member"]      = ("Thêm nhân viên vào cửa hàng", "Gán nhân viên vào cửa hàng"),
        ["hrm:stores:remove-member"]   = ("Xóa nhân viên khỏi cửa hàng", "Gỡ nhân viên ra khỏi cửa hàng"),
        ["hrm:stores:import-from-pos"] = ("Nhập cửa hàng từ POS", "Import dữ liệu cửa hàng từ hệ thống POS"),

        // ── Quản lý cửa hàng (Store Manager) ─────────────────────────────
        ["hrm:store-manager:view-store"]   = ("Xem thông tin cửa hàng phụ trách", "Xem chi tiết cửa hàng mà quản lý đang phụ trách"),
        ["hrm:store-manager:view-members"] = ("Xem nhân viên cửa hàng phụ trách", "Xem danh sách nhân viên trong cửa hàng phụ trách"),

        // ── Khu vực ───────────────────────────────────────────────────────
        ["hrm:regions:view-list"]    = ("Xem danh sách khu vực", "Xem toàn bộ danh sách khu vực"),
        ["hrm:regions:sync"]         = ("Đồng bộ khu vực", "Đồng bộ dữ liệu khu vực từ hệ thống ngoài"),
        ["hrm:regions:view-hours"]   = ("Xem giờ làm việc khu vực", "Xem lịch giờ hoạt động của khu vực"),
        ["hrm:regions:update-hours"]   = ("Cập nhật giờ làm việc khu vực", "Chỉnh sửa lịch giờ hoạt động khu vực"),
        ["hrm:regions:assign-manager"] = ("Gán quản lý khu vực", "Gán hoặc gỡ người quản lý cho khu vực"),

        // ── Quầy hàng ─────────────────────────────────────────────────────
        ["hrm:counters:view-list"] = ("Xem danh sách quầy hàng", "Xem toàn bộ danh sách quầy hàng"),
        ["hrm:counters:create"]    = ("Tạo quầy hàng", "Tạo mới quầy hàng"),
        ["hrm:counters:update"]    = ("Cập nhật quầy hàng", "Chỉnh sửa thông tin quầy hàng"),
        ["hrm:counters:delete"]    = ("Xóa quầy hàng", "Xóa quầy hàng"),

        // ── Trường tùy chỉnh ──────────────────────────────────────────────
        ["hrm:custom-fields:view-list"]   = ("Xem danh sách trường tùy chỉnh", "Xem toàn bộ trường mở rộng trong hệ thống"),
        ["hrm:custom-fields:view-detail"] = ("Xem chi tiết trường tùy chỉnh", "Xem thông tin chi tiết một trường tùy chỉnh"),
        ["hrm:custom-fields:create"]      = ("Tạo trường tùy chỉnh", "Tạo mới trường mở rộng"),
        ["hrm:custom-fields:update"]      = ("Cập nhật trường tùy chỉnh", "Chỉnh sửa trường mở rộng"),
        ["hrm:custom-fields:delete"]      = ("Xóa trường tùy chỉnh", "Xóa trường mở rộng"),

        // ── Tài liệu nhân sự ─────────────────────────────────────────────
        ["hrm:documents:view"]   = ("Xem tài liệu nhân sự", "Xem tài liệu đính kèm hồ sơ nhân sự"),
        ["hrm:documents:upload"] = ("Tải lên tài liệu", "Đính kèm tài liệu vào hồ sơ nhân sự"),
        ["hrm:documents:delete"] = ("Xóa tài liệu", "Xóa tài liệu khỏi hồ sơ nhân sự"),

        // ── Nhãn hồ sơ nhân sự ───────────────────────────────────────────
        ["hrm:labels:view"]   = ("Xem nhãn hồ sơ", "Xem danh sách nhãn và nhãn gắn trên hồ sơ nhân sự"),
        ["hrm:labels:manage"] = ("Quản lý nhãn", "Tạo, sửa, xóa nhãn hồ sơ nhân sự"),
        ["hrm:labels:assign"] = ("Gán nhãn nhân sự", "Gán hoặc gỡ nhãn khỏi hồ sơ nhân sự"),

        // ── Cấu hình tuyển dụng ───────────────────────────────────────────
        ["hrm:recruitment:approver-config:view"]   = ("Xem cấu hình người duyệt tuyển dụng", "Xem danh sách người duyệt phiếu đề xuất tuyển dụng"),
        ["hrm:recruitment:approver-config:manage"] = ("Quản lý cấu hình người duyệt tuyển dụng", "Thêm, sửa, xóa cấu hình người duyệt phiếu đề xuất tuyển dụng"),

        // ── Lương cơ bản ──────────────────────────────────────────────────
        ["hrm:salary:view"] = ("Xem lương nhân sự", "Xem mức lương của nhân sự"),
        ["hrm:salary:set"]  = ("Thiết lập lương nhân sự", "Cập nhật mức lương cho nhân sự"),

        // ── Bảng lương ───────────────────────────────────────────────────
        ["hrm:payroll-runs:view-list"]    = ("Xem danh sách bảng lương", "Xem toàn bộ các kỳ bảng lương"),
        ["hrm:payroll-runs:view-detail"]  = ("Xem chi tiết bảng lương", "Xem thông tin chi tiết một kỳ bảng lương"),
        ["hrm:payroll-runs:create"]       = ("Tạo bảng lương", "Khởi tạo kỳ tính lương mới"),
        ["hrm:payroll-runs:update-entry"] = ("Cập nhật dòng lương", "Chỉnh sửa thông tin lương từng nhân sự trong kỳ"),
        ["hrm:payroll-runs:finalize"]     = ("Chốt bảng lương", "Xác nhận và khóa kỳ bảng lương"),

        // ── Mẫu KPI ───────────────────────────────────────────────────────
        ["hrm:kpi-templates:view-list"]   = ("Xem danh sách mẫu KPI", "Xem toàn bộ mẫu đánh giá KPI"),
        ["hrm:kpi-templates:view-detail"] = ("Xem chi tiết mẫu KPI", "Xem thông tin chi tiết một mẫu KPI"),
        ["hrm:kpi-templates:create"]      = ("Tạo mẫu KPI", "Tạo mới mẫu đánh giá KPI"),
        ["hrm:kpi-templates:update"]      = ("Cập nhật mẫu KPI", "Chỉnh sửa mẫu KPI"),
        ["hrm:kpi-templates:delete"]      = ("Xóa mẫu KPI", "Xóa mẫu KPI"),

        // ── Đánh giá KPI ─────────────────────────────────────────────────
        ["hrm:kpi-entries:view-list"]    = ("Xem danh sách KPI", "Xem danh sách kết quả đánh giá KPI"),
        ["hrm:kpi-entries:view-summary"] = ("Xem tổng hợp KPI", "Xem báo cáo tổng hợp kết quả KPI"),
        ["hrm:kpi-entries:upsert"]       = ("Nhập / cập nhật KPI", "Tạo mới hoặc cập nhật kết quả đánh giá KPI"),

        // ── Hợp đồng lao động ─────────────────────────────────────────────
        ["hrm:contract:view"]      = ("Xem hợp đồng", "Xem thông tin hợp đồng lao động của nhân sự"),
        ["hrm:contract:create"]    = ("Tạo hợp đồng", "Tạo mới hợp đồng lao động"),
        ["hrm:contract:renew"]     = ("Gia hạn hợp đồng", "Gia hạn hợp đồng lao động sắp hết hạn"),
        ["hrm:contract:terminate"] = ("Chấm dứt hợp đồng", "Kết thúc hợp đồng lao động"),

        // ── Mẫu hợp đồng ─────────────────────────────────────────────────
        ["hrm:contract-templates:view"]     = ("Xem mẫu hợp đồng", "Xem danh sách và nội dung mẫu hợp đồng"),
        ["hrm:contract-templates:upload"]   = ("Tải lên mẫu hợp đồng", "Đính kèm file mẫu hợp đồng"),
        ["hrm:contract-templates:download"] = ("Tải xuống mẫu hợp đồng", "Tải file mẫu hợp đồng về máy"),
        ["hrm:contract-templates:delete"]   = ("Xóa mẫu hợp đồng", "Xóa mẫu hợp đồng khỏi hệ thống"),

        // ── Chính sách thưởng ─────────────────────────────────────────────
        ["hrm:bonus-policies:view-list"] = ("Xem chính sách thưởng", "Xem danh sách chính sách thưởng hiện hành"),
        ["hrm:bonus-policies:create"]    = ("Tạo chính sách thưởng", "Tạo mới chính sách thưởng"),
    };
}
