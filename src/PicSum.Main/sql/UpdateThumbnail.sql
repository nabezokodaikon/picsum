UPDATE t_thumbnail
   SET thumbnail_buffer = :thumbnail_buffer
      ,thumbnail_width  = :thumbnail_width
      ,thumbnail_height = :thumbnail_height
      ,source_width     = :source_width
      ,source_height    = :source_height
      ,file_update_date = :file_update_date
 WHERE file_path = :file_path