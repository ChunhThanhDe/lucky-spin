### 1\. Mục đích của file INPUT.xlsx



File INPUT.xlsx dùng để cấu hình danh sách kết quả cho vòng quay Lucky Spin, bao gồm:



Tên nội dung hiển thị (có thể là tên người, số tiền, hoặc chuỗi bất kỳ)



Thiết lập kịch bản trúng cố định theo lượt quay (STT)



Thiết lập xác suất trúng thưởng theo tỷ lệ (RATE)



Quy định kết quả có bị loại sau khi trúng hay không



-------------------------------------------------------------------------------------------------------------



### 2\. Cấu trúc các cột trong INPUT.xlsx

Cột	Tên	Bắt buộc	Ý nghĩa

A	NAME	✔	Nội dung hiển thị khi trúng (tên / số tiền / text)

B	STT	❌	Lần quay bắt buộc trúng (1-based)

C	RATE	❌	Tỷ lệ xác suất trúng (trọng số)

D	ISCLEAR	❌	Trúng xong có loại khỏi vòng quay hay không

E	IMAGE	❌	Tên file ảnh trong thư mục Images (ví dụ: money1.png)

-------------------------------------------------------------------------------------------------------------



### 3\. Giải thích chi tiết từng cột

#### 3.1. Cột NAME



Nội dung sẽ hiển thị khi quay trúng



Có thể là:



Tên người



Số tiền



Mã giải thưởng



Không ảnh hưởng đến thuật toán, chỉ để hiển thị



📌 Bắt buộc phải có giá trị



#### 3.2. Cột STT – Ép cứng theo kịch bản

Giá trị STT	Ý nghĩa

Để trống	Không ép

Số nguyên dương (1,2,3,…)	Lần quay đó trúng chắc 100%



📌 Quy tắc:



STT được tính theo thứ tự lượt quay



STT ưu tiên tuyệt đối, không bị ảnh hưởng bởi RATE



Mỗi STT chỉ nên dùng một lần



📌 Ví dụ:



NAME	STT

100.000	3



→ Lần quay thứ 3 chắc chắn trúng 100.000



#### 3.3. Cột RATE – Xác suất trúng (random có kiểm soát)

Giá trị RATE	Ý nghĩa

> 0	Tham gia random với trọng số

= 0	❌ Không bao giờ trúng

Trống / < 0	✔ Mặc định = 1



📌 Lưu ý quan trọng:



RATE không phải %



RATE chỉ là trọng số tương đối



Xác suất thực tế = RATE / tổng RATE



📌 Ví dụ:



NAME	RATE

A	(trống)

B	5

C	10



→ RATE = \[1, 5, 10]

→ Tổng = 16

→ Xác suất:



A ≈ 6.25%



B ≈ 31.25%



C ≈ 62.5%



#### 3.4. Cột ISCLEAR – Loại sau khi trúng

ISCLEAR	Ý nghĩa

TRUE	Trúng xong → loại khỏi vòng quay

FALSE / trống	Có thể trúng lại



📌 Khuyến nghị:



Giải thưởng → TRUE



Tên người (quay nhiều vòng) → FALSE

#### 3.5. Cột IMAGE – Hiển thị ảnh

Giá trị IMAGE	Ý nghĩa

Tên file (ví dụ: money1.png)	Hiển thị ảnh trên vòng quay và khi trúng

Trống	Không hiển thị ảnh



📌 Lưu ý quan trọng:



File ảnh phải được đặt trong thư mục **Images** (cùng cấp với thư mục Config)



Chỉ cần nhập tên file, không cần đường dẫn đầy đủ



Hỗ trợ các định dạng: .png, .jpg, .jpeg, .gif



Kích thước khuyến nghị: 256x256px hoặc 512x512px



📌 Ví dụ:



NAME	IMAGE

1.000.000 VNĐ	money2.png

500.000 VNĐ	money1.png

Quà tặng đặc biệt	gift1.png



→ Các file money2.png, money1.png, gift1.png phải nằm trong thư mục **Images/**

-------------------------------------------------------------------------------------------------------------



### 4\. Thứ tự ưu tiên thuật toán (rất quan trọng)



Mỗi lần quay, hệ thống xử lý theo thứ tự sau:



1\. Nếu có STT == lượt quay hiện tại

&nbsp;     → Trúng chắc kết quả đó (100%)



2\. Nếu không có STT

&nbsp;     → Random theo RATE



3\. Nếu RATE trống hết

&nbsp;     → Random truyền thống (chia đều)





👉 STT luôn override RATE

-------------------------------------------------------------------------------------------------------------



### 5\. Các kịch bản sử dụng phổ biến

🔹 Kịch bản 1: Random hoàn toàn



STT: trống



RATE: trống



→ Quay đều 100%



🔹 Kịch bản 2: Random có kiểm soát



STT: trống



RATE: có số



→ Giải lớn ít trúng, giải nhỏ dễ trúng



🔹 Kịch bản 3: Kịch bản cố định + random



STT: dùng cho vài lượt đặc biệt



RATE: dùng cho các lượt còn lại



→ Phổ biến nhất cho event / gala

-------------------------------------------------------------------------------------------------------------



### 6\. Những lưu ý QUAN TRỌNG



⚠️ Không nên:



Dùng STT trùng nhau



Để tổng RATE = 0



Nhập RATE = 0 nếu vẫn muốn giải đó trúng



✔ Nên:



Dùng RATE trống cho giải thường



Dùng RATE lớn cho giải muốn ưu tiên



Dùng STT cho các lượt quan trọng

