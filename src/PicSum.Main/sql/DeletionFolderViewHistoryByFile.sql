/* TODO: t_folder_view_counter を実装したら、削除する。 */
DELETE FROM t_folder_view_history
 WHERE file_id = (SELECT mf.file_id
                    FROM m_file mf
                   WHERE mf.file_path = :file_path
                 )