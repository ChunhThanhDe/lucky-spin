# HƯỚNG DẪN BUILD LẠI ỨNG DỤNG

## Đã thực hiện các thay đổi:

✅ **Thêm tính năng hiển thị ảnh:**
1. Thêm cột IMAGE vào file Excel
2. Hiển thị ảnh trên các mảnh ghép của vòng quay
3. Hiển thị ảnh khi trúng thưởng
4. Tạo 4 ảnh mẫu trong thư mục Images:
   - money1.png (đồng tiền vàng)
   - money2.png (chồng tiền vàng)
   - gift1.png (hộp quà đỏ)
   - sad.png (mặt buồn)
5. Tạo file Excel mẫu với cột IMAGE

## Để sử dụng ngay:

### Cách 1: Build bằng Visual Studio (Khuyến nghị)

1. Mở file `LuckySpin.sln` bằng Visual Studio
2. Chọn **Build** > **Rebuild Solution** (hoặc nhấn `Ctrl+Shift+B`)
3. Chờ build hoàn thành
4. File .exe sẽ được tạo tại: `LuckySpin\LuckySpin\bin\Debug\LuckySpin.exe`
5. Copy toàn bộ nội dung trong `bin\Debug\` sang `bin\Lucky Wheel\` (ghi đè file cũ)

### Cách 2: Chạy trực tiếp từ Visual Studio

1. Mở file `LuckySpin.sln` bằng Visual Studio
2. Nhấn F5 hoặc nhấn nút **Start** để chạy
3. Ứng dụng sẽ tự động build và chạy

### Cách 3: Build bằng command line

Mở **Developer Command Prompt for VS** và chạy:

```cmd
cd "e:\Lucky Spin\Lucky Spin\LuckySpin"
msbuild LuckySpin.sln /p:Configuration=Debug /t:Rebuild
```

## Kiểm tra file Excel:

File Excel mẫu đã được tạo tại:
```
e:\Lucky Spin\Lucky Spin\LuckySpin\LuckySpin\bin\Lucky Wheel\Config\INPUT.xlsx
```

Cấu trúc file:

| NAME | STT | RATE | ISCLEAR | IMAGE |
|------|-----|------|---------|-------|
| 1.000.000 VNĐ | 10 | | TRUE | money2.png |
| 500.000 VNĐ | | | TRUE | money1.png |
| 100.000 VNĐ | | | TRUE | gift1.png |
| 50.000 VNĐ | | | TRUE | money1.png |
| Chúc may mắn lần sau | | | FALSE | sad.png |
| 20.000 VNĐ | | | TRUE | gift1.png |
| 10.000 VNĐ | | | TRUE | money1.png |

## Thư mục Images:

Các file ảnh mẫu nằm tại:
```
e:\Lucky Spin\Lucky Spin\LuckySpin\LuckySpin\bin\Lucky Wheel\Images\
├── money1.png
├── money2.png
├── gift1.png
└── sad.png
```

Bạn có thể thêm ảnh của riêng mình vào thư mục này!

## Lưu ý:

- Ảnh nên có kích thước 256x256px hoặc 512x512px
- Định dạng hỗ trợ: .png, .jpg, .jpeg, .gif
- Tên file trong Excel phải khớp với tên file trong thư mục Images
- Nếu không có ảnh, để trống cột IMAGE trong Excel

## Test ngay:

1. Build lại ứng dụng
2. Chạy `LuckySpin.exe`
3. Nhấn nút SPIN để xem ảnh hiển thị trên vòng quay
4. Khi trúng, popup sẽ hiển thị ảnh to ở giữa màn hình!

🎉 Chúc bạn thành công!
