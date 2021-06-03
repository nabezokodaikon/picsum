SELECT tt.tag
      ,CASE COUNT(1) WHEN :file_count THEN 'TRUE'
                     ELSE 'FALSE'
        END AS is_all
  FROM m_file mf
       INNER JOIN t_tag tt
          ON tt.file_id = mf.file_id
 WHERE {mf.file_path = :file_path}
 GROUP BY tt.tag