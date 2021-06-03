SELECT tfvh.view_date
  FROM t_file_view_history tfvh
 GROUP BY DATE(tfvh.view_date)
