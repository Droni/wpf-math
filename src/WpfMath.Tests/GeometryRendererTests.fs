namespace WpfMath.Tests

open System
open System.Windows
open System.Windows.Media

open Foq
open Xunit

open WpfMath
open WpfMath.Rendering

type GeometryRendererTests() =
    static do Utils.initializeFontResourceLoading()

    let geometry = GeometryGroup()
    let renderer = GeometryRenderer(1.0, geometry) :> IElementRenderer

    [<Fact>]
    member __.``GeometryRenderer.RenderElement delegates to element.RenderTo``() : unit =
        let box = Mock.Of<Box>()
        renderer.RenderElement(box, 1.0, 2.0)
        Mock.Verify(<@ box.RenderTo(renderer, 1.0, 2.0) @>, once)

    [<Fact>]
    member __.``GeometryRenderer.RenderGlyphRun adds a PathGeometry group``() : unit =
        let font = DefaultTexFont 20.0
        let environment = TexEnvironment(TexStyle.Display, font, font)
        let char = environment.MathFont.GetDefaultCharInfo('x', TexStyle.Display)
        let charBox = CharBox(environment, char)
        let glyphRun = charBox.GetGlyphRun(1.0, 0.0, 0.0)
        let factory = Func<double, GlyphRun>(fun s -> glyphRun)
        renderer.RenderGlyphRun(factory, 0.0, 0.0, Brushes.Black)

        let group = Seq.exactlyOne geometry.Children :?> GeometryGroup
        Assert.IsType<PathGeometry>(Seq.exactlyOne group.Children) |> ignore

    [<Fact>]
    member __.``GeometryRenderer.RenderRectangle adds a RectangleGeometry``() : unit =
        let rect = Rect(1.0, 2.0, 3.0, 4.0)
        renderer.RenderRectangle(rect, null)

        Assert.IsType<RectangleGeometry>(Seq.exactlyOne geometry.Children) |> ignore

    [<Fact>]
    member __.``GeometryRenderer.RenderTransformed adds a GeometryGroup``() : unit =
        renderer.RenderTransformed(HorizontalBox(), [| TranslateTransform(1.0, 1.0) |], 0.0, 0.0)
        Assert.IsType<GeometryGroup>(Seq.exactlyOne geometry.Children) |> ignore
