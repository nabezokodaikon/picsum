SELECT tt.file_path
      ,tt.thumbnail_buffer
      ,tt.thumbnail_width
      ,tt.thumbnail_height
      ,tt.source_width
      ,tt.source_height
      ,tt.file_update_date
  FROM t_thumbnail tt
 WHERE tt.file_path = :file_path