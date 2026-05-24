INSERT INTO CATEGORIES (category_name, description) VALUES
('Vợt cầu lông', 'Các loại vợt thi đấu, luyện tập'),
('Giày cầu lông', 'Giày chuyên dụng cho cầu lông'),
('Phụ kiện', 'Bao vợt, quấn cán, áo cầu lông, tất…');

INSERT INTO PRODUCTS (category_id, product_name, brand, price, stock, image_url, description) VALUES
(1, 'Yonex Astrox 100ZZ', 'Yonex', 3500000, 20, 'https://example.com/astrox100zz.jpg', 'Vợt tấn công chuyên nghiệp, công thủ toàn diện'),
(1, 'Victor Thruster K Falcon', 'Victor', 2900000, 15, 'https://example.com/thrusterfalcon.jpg', 'Vợt thiên về smash mạnh mẽ'),
(2, 'Yonex SHB 65Z3', 'Yonex', 2200000, 30, 'https://example.com/shb65z3.jpg', 'Giày thi đấu Yonex, êm ái và bám sân'),
(2, 'Mizuno Wave Claw 2', 'Mizuno', 2000000, 25, 'https://example.com/waveclaw2.jpg', 'Giày cầu lông Mizuno nhẹ, hỗ trợ di chuyển nhanh'),
(3, 'Bao vợt cầu lông 6 ngăn', 'Yonex', 900000, 10, 'https://example.com/baovot6.jpg', 'Bao vợt lớn, đựng 6 vợt + phụ kiện'),
(3, 'Cuốn cán vợt Karakal PU Super Grip', 'Karakal', 50000, 100, 'https://example.com/karakalgrip.jpg', 'Quấn cán siêu bám, thoáng khí');

INSERT INTO USERS (full_name, email, password_hash, phone, address, role) VALUES
('Nguyễn Văn A', 'a@example.com', 'hashed_password_123', '0901234567', 'TP.HCM', 'customer'),
('Trần Thị B', 'b@example.com', 'hashed_password_456', '0912345678', 'Hà Nội', 'customer'),
('Admin Store', 'admin@badmintonstore.com', 'hashed_admin_pass', '0999999999', 'TP.HCM', 'admin');

INSERT INTO ORDERS (user_id, status, total_amount) VALUES
(1, 'confirmed', 3500000),
(2, 'pending', 5000000);

INSERT INTO ORDER_DETAILS (order_id, product_id, quantity, price) VALUES
(1, 1, 1, 3500000), -- Nguyễn Văn A mua Yonex Astrox 100ZZ
(2, 2, 1, 2900000), -- Trần Thị B mua Victor Thruster K Falcon
(2, 3, 1, 2200000); -- + giày Yonex SHB 65Z3

INSERT INTO PAYMENTS (order_id, payment_method, payment_status) VALUES
(1, 'credit_card', 'paid'),
(2, 'cod', 'pending');

INSERT INTO SHIPMENTS (order_id, tracking_number, courier, status) VALUES
(1, 'J&T123456', 'J&T Express', 'delivered'),
(2, 'GHN987654', 'Giao Hàng Nhanh', 'shipping');

INSERT INTO REVIEWS (user_id, product_id, rating, comment) VALUES
(1, 1, 5, 'Vợt Yonex đánh rất đầm tay, smash mạnh!'),
(2, 3, 4, 'Giày Yonex mang thoải mái, bám sân tốt.');
