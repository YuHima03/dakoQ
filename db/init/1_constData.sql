INSERT INTO `room_data_sources` (`id`, `name`, `is_active`) VALUES
    (1, 'dakoQ', 1),
    (2, 'knoQ_room', 1),
    (3, 'knoQ_event', 1);

INSERT INTO `rooms` (`id`, `name`, `data_source_id`, `alias`, `starts_at`, `ends_at`, `created_at`, `updated_at`) VALUES
    (uuid(), '部室', '1', 'bu', NULL, NULL, now(), now()),
    (uuid(), '滝プラザ', '1', 'taki', NULL, NULL, now(), now()),
    (uuid(), '大岡山図書館', '1', 'o-lib', NULL, NULL, now(), now()),
    (uuid(), 'すずかけ台図書館', '1', 's-lib', NULL, NULL, now(), now());
