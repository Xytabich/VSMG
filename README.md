# VSMG
Generator of simple models for VS Model Creator. Current generators only create basic shapes and further refinement can be done in VSMC.

![img](preview.png)

### How it works
A generator consists of a main application and generator plug-ins. Dll generators located in the `generators` folder are connected at application start.

In the application you can select a generator and configure it. Additionally you can customize textures, materials and root object.
Materials contain settings for vertices and faces.

If you want to save the settings of generators or shape - you can use the menu `File/Save generator|shape preset`. Presets can be loaded via the `File/Load preset(s)` menu. Through this menu you can load new or update current presets, you can also select several files at once. Loaded presets will be available through the `Copy from preset...` drop-down.

To export the generated model file click "Generate".

There is a lot of metadata in json since the serializer saves all the fields. So it is recommended to re-save the model through VSMC.

### Sphere generator
It is possible to choose:
- Radius
- Even
- Offset
The first texture is selected.
An additional option is a hollow sphere, you can choose an inner radius.

### Cylinder generator
The radius, parity and offset settings are the same as for the sphere.
- Length
- Vertical axis
It also has a hollow version (i.e. a pipe).

### Custom generators
WPF is used for UI. The generator must be marked with the `ShapeGenerator(...)` attribute and inherit from `IShapeGenerator`.

When the generator is selected `ShowPanel(EditorContext)` is called. You can add the necessary controls to the `parent`. When switching to another generator `OnHide()` will be called.

When the `Generate` button is pressed the `Generate(GeneratorContext)` method will be called. You can add any parameters to the shape. When the method is called the shape already contains the texture settings. If root was enabled all shape elements will be moved in root before export.

To add your own generator just drop the dll into the `generators` folder.
