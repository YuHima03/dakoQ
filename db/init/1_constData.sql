INSERT INTO `room_data_sources` (`id`, `name`, `is_active`) VALUES
    (1, 'Dakoq', 1),
    (2, 'KnoqRoom', 1),
    (3, 'KnoqEvent', 1);

INSERT INTO `rooms` (`id`, `name`, `data_source_id`, `source_id`, `alias`, `starts_at`, `ends_at`, `created_at`, `updated_at`) VALUES
    (uuid(), '部室', '1', NULL, 'bu', NULL, NULL, now(), now()),
    (uuid(), '滝プラザ', '1', NULL, 'taki', NULL, NULL, now(), now()),
    (uuid(), '大岡山図書館', '1', NULL, 'o-lib', NULL, NULL, now(), now()),
    (uuid(), 'すずかけ台図書館', '1', NULL, 's-lib', NULL, NULL, now(), now());
