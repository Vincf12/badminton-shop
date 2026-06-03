START TRANSACTION;

INSERT INTO users (user_id, email, password_hash, full_name, phone, avatar_url, gender, birthdate, role, is_active, email_verified, created_at, updated_at) VALUES
(1, 'a@example.com', 'hashed_password_123', 'Nguyễn Văn A', '0901234567', NULL, 'male', '1995-01-15', 'customer', TRUE, TRUE, '2026-05-29 08:00:00', NULL),
(2, 'b@example.com', 'hashed_password_456', 'Trần Thị B', '0912345678', NULL, 'female', '1997-03-22', 'customer', TRUE, FALSE, '2026-05-29 08:05:00', NULL),
(3, 'admin@badmintonstore.com', 'hashed_admin_pass', 'Admin Store', '0999999999', NULL, 'other', NULL, 'admin', TRUE, TRUE, '2026-05-29 08:10:00', NULL);

INSERT INTO addresses (address_id, user_id, recipient_name, phone, province, district, ward, address_detail, is_default) VALUES
(1, 1, 'Nguyễn Văn A', '0901234567', 'Hà Nội', 'Cầu Giấy', 'Dịch Vọng Hậu', 'Số 12, ngõ 34, đường Trần Thái Tông', TRUE),
(2, 2, 'Trần Thị B', '0912345678', 'TP. Hồ Chí Minh', 'Quận 1', 'Bến Nghé', '25 Nguyễn Huệ, Phường Bến Nghé', TRUE),
(3, 3, 'Admin Store', '0999999999', 'Hà Nội', 'Đống Đa', 'Láng Hạ', '55 Láng Hạ, tầng 3', FALSE);

INSERT INTO categories (category_id, category_name, description) VALUES
(1, 'Vợt cầu lông', 'Các loại vợt thi đấu, luyện tập'),
(2, 'Giày cầu lông', 'Giày chuyên dụng cho cầu lông'),
(3, 'Phụ kiện', 'Bao vợt, quấn cán, áo cầu lông, tất…');

INSERT INTO brands (brand_id, brand_name, logo_url) VALUES
(1, 'Yonex', 'https://example.com/logos/yonex.png'),
(2, 'Victor', 'https://example.com/logos/victor.png'),
(3, 'Mizuno', 'https://example.com/logos/mizuno.png'),
(4, 'Karakal', 'https://example.com/logos/karakal.png');

INSERT INTO products (product_id, category_id, brand_id, product_name, slug, short_description, description, status, created_at, updated_at) VALUES
(1, 1, 1, 'Yonex Astrox 100ZZ', 'yonex-astrox-100zz', 'Vợt tấn công cao cấp cho người chơi chuyên nghiệp.', 'Yonex Astrox 100ZZ là dòng vợt cao cấp dành cho người chơi thiên công, cân bằng tốt giữa tốc độ và sức mạnh.', 'active', '2026-05-29 08:20:00', NULL),
(2, 1, 2, 'Victor Thruster K Falcon', 'victor-thruster-k-falcon', 'Vợt thiên smash mạnh, phù hợp lối đánh áp đảo.', 'Victor Thruster K Falcon hỗ trợ lực đập mạnh, thích hợp cho người chơi có thiên hướng tấn công.', 'active', '2026-05-29 08:21:00', NULL),
(3, 2, 1, 'Yonex SHB 65Z3', 'yonex-shb-65z3', 'Giày thi đấu êm ái, bám sân tốt.', 'Yonex SHB 65Z3 mang lại độ êm và độ ổn định cao, phù hợp thi đấu cường độ lớn.', 'active', '2026-05-29 08:22:00', NULL),
(4, 2, 3, 'Mizuno Wave Claw 2', 'mizuno-wave-claw-2', 'Giày nhẹ, hỗ trợ di chuyển nhanh.', 'Mizuno Wave Claw 2 là lựa chọn phổ biến cho người chơi cần sự linh hoạt và bám sân.', 'active', '2026-05-29 08:23:00', NULL),
(5, 3, 1, 'Bao vợt cầu lông 6 ngăn', 'bao-vot-cau-long-6-ngan', 'Bao đựng vợt dung lượng lớn.', 'Bao vợt lớn, đựng 6 vợt + phụ kiện, phù hợp di chuyển và thi đấu.', 'hidden', '2026-05-29 08:24:00', NULL),
(6, 3, 4, 'Cuốn cán vợt Karakal PU Super Grip', 'cuon-can-vot-karakal-pu-super-grip', 'Quấn cán siêu bám, thoáng khí.', 'Cuốn cán Karakal PU Super Grip mang lại cảm giác bám tay tốt và thoáng khí.', 'active', '2026-05-29 08:25:00', NULL);

INSERT INTO product_variants (variant_id, product_id, sku, weight, grip_size, color, price, stock_quantity, image_url) VALUES
(1, 1, 'ASTROX100ZZ-4U', '4U', 'G5', 'Xanh', 3500000.00, 8, 'https://example.com/astrox100zz-4u.jpg'),
(2, 1, 'ASTROX100ZZ-3U', '3U', 'G5', 'Đen', 3500000.00, 12, 'https://example.com/astrox100zz-3u.jpg'),
(3, 2, 'THRUSTERKFALCON-4U', '4U', 'G5', 'Đỏ', 2900000.00, 15, 'https://example.com/thruster-falcon-4u.jpg'),
(4, 3, 'SHB65Z3-41', '41', NULL, 'Trắng', 2200000.00, 10, 'https://example.com/shb65z3-41.jpg'),
(5, 4, 'WAVECLAW2-40', '40', NULL, 'Đen', 2000000.00, 10, 'https://example.com/waveclaw2-40.jpg'),
(6, 5, 'BAOVOT-6NGAN', NULL, NULL, 'Đen', 900000.00, 10, 'https://example.com/baovot-6-ngan.jpg'),
(7, 6, 'PU-SUPER-GRIP', NULL, NULL, 'Đỏ', 50000.00, 100, 'https://example.com/pu-super-grip.jpg');

INSERT INTO product_images (image_id, product_id, image_url, is_main, sort_order) VALUES
(1, 1, 'https://example.com/astrox100zz-main.jpg', TRUE, 1),
(2, 1, 'https://example.com/astrox100zz-side.jpg', FALSE, 2),
(3, 2, 'https://example.com/thruster-main.jpg', TRUE, 1),
(4, 3, 'https://example.com/shb65z3-main.jpg', TRUE, 1),
(5, 4, 'https://example.com/waveclaw2-main.jpg', TRUE, 1),
(6, 5, 'https://example.com/baovot-main.jpg', TRUE, 1),
(7, 6, 'https://example.com/pu-grip-main.jpg', TRUE, 1);

INSERT INTO product_specs (spec_id, product_id, spec_name, spec_value) VALUES
(1, 1, 'Trọng lượng', '4U'),
(2, 1, 'Độ cứng', 'Cứng'),
(3, 1, 'Điểm cân bằng', 'Nặng đầu'),
(4, 3, 'Chất liệu', 'Lưới thoáng khí'),
(5, 3, 'Đối tượng', 'Thi đấu'),
(6, 4, 'Công nghệ', 'Wave hỗ trợ lực'),
(7, 6, 'Chất liệu', 'PU');

INSERT INTO carts (cart_id, user_id, created_at) VALUES
(1, 1, '2026-05-29 08:40:00'),
(2, 2, '2026-05-29 08:45:00');

INSERT INTO cart_items (cart_item_id, cart_id, variant_id, quantity) VALUES
(1, 1, 1, 1),
(2, 1, 7, 2),
(3, 2, 4, 1);

INSERT INTO coupons (coupon_id, code, discount_type, discount_value, max_discount, min_order_value, start_date, end_date, usage_limit, is_active) VALUES
(1, 'SUMMER2026', 'percentage', 10.00, 500000.00, 3000000.00, '2026-05-01 00:00:00', '2026-08-31 23:59:59', 100, TRUE);

INSERT INTO orders (order_id, user_id, coupon_id, shipping_recipient_name, shipping_phone, shipping_province, shipping_district, shipping_ward, shipping_address_detail, total_amount, shipping_fee, discount_amount, final_amount, status, created_at) VALUES
(1, 1, NULL, 'Nguyễn Văn A', '0901234567', 'Hà Nội', 'Cầu Giấy', 'Dịch Vọng Hậu', 'Số 12, ngõ 34, đường Trần Thái Tông', 3500000.00, 30000.00, 0.00, 3530000.00, 'confirmed', '2026-05-29 09:00:00'),
(2, 2, 1, 'Trần Thị B', '0912345678', 'TP. Hồ Chí Minh', 'Quận 1', 'Bến Nghé', '25 Nguyễn Huệ, Phường Bến Nghé', 5100000.00, 30000.00, 500000.00, 4630000.00, 'shipping', '2026-05-29 10:15:00');

INSERT INTO order_details (order_detail_id, order_id, variant_id, quantity, unit_price, subtotal) VALUES
(1, 1, 1, 1, 3500000.00, 3500000.00),
(2, 2, 3, 1, 2900000.00, 2900000.00),
(3, 2, 4, 1, 2200000.00, 2200000.00);

INSERT INTO payments (payment_id, order_id, payment_method, payment_status, transaction_code, paid_at) VALUES
(1, 1, 'cod', 'paid', 'COD-20260529-0001', '2026-05-29 09:30:00'),
(2, 2, 'vnpay', 'pending', 'VN20260529-0001', NULL);

INSERT INTO shipments (shipment_id, order_id, tracking_number, courier, shipped_date, delivered_date, status) VALUES
(1, 1, 'J&T123456', 'J&T Express', '2026-05-29 10:00:00', '2026-05-30 15:00:00', 'delivered'),
(2, 2, 'GHN987654', 'Giao Hàng Nhanh', '2026-05-29 12:00:00', NULL, 'shipping');

INSERT INTO order_status_history (history_id, order_id, status, note, created_at) VALUES
(1, 1, 'pending', 'Khởi tạo đơn hàng', '2026-05-29 09:00:00'),
(2, 1, 'confirmed', 'Đơn hàng đã được xác nhận', '2026-05-29 09:10:00'),
(3, 2, 'pending', 'Khởi tạo đơn hàng', '2026-05-29 10:15:00'),
(4, 2, 'confirmed', 'Đơn hàng đã được xác nhận', '2026-05-29 10:20:00'),
(5, 2, 'shipping', 'Đơn hàng đã bàn giao đơn vị vận chuyển', '2026-05-29 12:00:00');

INSERT INTO reviews (review_id, user_id, product_id, rating, comment, created_at) VALUES
(1, 1, 1, 5, 'Vợt Yonex đánh rất đầm tay, smash mạnh!', '2026-05-30 09:00:00'),
(2, 2, 3, 4, 'Giày Yonex mang thoải mái, bám sân tốt.', '2026-05-30 09:30:00');

INSERT INTO wishlists (wishlist_id, user_id) VALUES
(1, 1),
(2, 2);

INSERT INTO wishlist_items (wishlist_item_id, wishlist_id, product_id) VALUES
(1, 1, 1),
(2, 1, 5),
(3, 2, 3);

COMMIT;
