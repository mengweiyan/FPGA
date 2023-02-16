@ECHO OFF
wdreg -inf FT6678_YOLO_driver.inf install
if exist FT6678_YOLO_device.inf (
  wdreg -inf FT6678_YOLO_device.inf install
)

