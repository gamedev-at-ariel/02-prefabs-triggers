"""
Draws an original 8-frame animated spaceship sprite sheet (3200x400, frames of 400x400, ship facing +X).
Animation: flickering twin engine flames, blinking wing-tip navigation lights, a glint sweeping over the cockpit.
Usage: python draw_ship.py <output.png> [<preview.png>]
"""
import math, random, sys
from PIL import Image, ImageDraw, ImageFilter, ImageChops

FRAMES = 8
SIZE = 400
SS = 4                      # supersampling factor, for smooth anti-aliased edges
S = SIZE * SS

def P(points):
    return [(x * SS, y * SS) for x, y in points]

def mirror(points, cy=200):
    return [(x, 2 * cy - y) for x, y in points]

def bezier(p0, p1, p2, p3, n=24):
    out = []
    for i in range(n + 1):
        t = i / n
        a, b, c, d = (1 - t) ** 3, 3 * (1 - t) ** 2 * t, 3 * (1 - t) * t ** 2, t ** 3
        out.append((a * p0[0] + b * p1[0] + c * p2[0] + d * p3[0], a * p0[1] + b * p1[1] + c * p2[1] + d * p3[1]))
    return out

def mask_of(draw_fn):
    m = Image.new("L", (S, S), 0)
    draw_fn(ImageDraw.Draw(m))
    return m

def vertical_gradient(top, bottom, y0, y1):
    """An RGBA image whose color goes from `top` at y0 to `bottom` at y1 (in 400-px coordinates)."""
    g = Image.new("RGBA", (1, S))
    for y in range(S):
        t = min(1, max(0, (y / SS - y0) / (y1 - y0)))
        g.putpixel((0, y), tuple(int(top[i] + (bottom[i] - top[i]) * t) for i in range(4)))
    return g.resize((S, S))

def fill(canvas, mask, color_or_image):
    layer = color_or_image if isinstance(color_or_image, Image.Image) else Image.new("RGBA", (S, S), color_or_image)
    canvas.paste(layer, (0, 0), mask)

def outline(canvas, mask, width, color):
    grown = mask.filter(ImageFilter.MaxFilter(width * SS * 2 + 1))
    fill(canvas, ImageChops.subtract(grown, mask), color)

def glow(canvas, center, radius, color, strength=1.0):
    layer = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    d = ImageDraw.Draw(layer)
    cx, cy = center
    for i in range(12, 0, -1):
        r = radius * i / 12
        a = int(255 * strength * (1 - i / 12) ** 1.6)
        d.ellipse(P([(cx - r, cy - r), (cx + r, cy + r)]), fill=color + (a,))
    canvas.alpha_composite(layer.filter(ImageFilter.GaussianBlur(3 * SS)))

# ---------- Geometry (in 400x400 coordinates, ship nose pointing right) ----------
CY = 200
fuselage_top = (bezier((70, 178), (150, 150), (290, 152), (388, 200))
                + [(388, 200)])
fuselage = fuselage_top + mirror(fuselage_top[::-1])

wing_top = [(255, 182), (150, 74), (118, 66), (104, 74), (128, 182)]
wing_bottom = mirror(wing_top)
wing_stripe_top = [(196, 136), (160, 100), (140, 100), (170, 136)]
wing_stripe_bottom = mirror(wing_stripe_top)

fin_top = [(150, 165), (98, 120), (82, 122), (100, 170)]
fin_bottom = mirror(fin_top)

nacelle_top = [(58, 160), (150, 160), (160, 172), (150, 184), (58, 184)]
nacelle_bottom = mirror(nacelle_top)
ENGINE_Y = (172, 228)

cockpit_box = [(268, 184), (352, 216)]
hull_stripe = bezier((120, 196), (200, 192), (300, 192), (372, 200)) + bezier((372, 200), (300, 208), (200, 208), (120, 204))

NAV_LIGHTS = [((112, 70), (255, 70, 70)), ((112, 330), (70, 255, 120))]

# ---------- Colors ----------
OUTLINE = (16, 22, 40, 255)
HULL_TOP, HULL_BOTTOM = (236, 242, 250, 255), (122, 138, 168, 255)
WING_TOP, WING_BOTTOM = (196, 208, 228, 255), (96, 110, 142, 255)
ACCENT = (40, 170, 230, 255)
ACCENT_DARK = (24, 104, 170, 255)
NACELLE = (70, 80, 104, 255)
NOZZLE = (40, 44, 60, 255)
GLASS_TOP, GLASS_BOTTOM = (150, 236, 255, 255), (20, 80, 150, 255)

def draw_frame(i):
    rnd = random.Random(1000 + i)
    canvas = Image.new("RGBA", (S, S), (0, 0, 0, 0))

    # Engine flames (behind everything). Length pulses smoothly and flickers a little.
    pulse = 0.5 + 0.5 * math.sin(2 * math.pi * i / FRAMES)
    for ey in ENGINE_Y:
        length = 34 + 22 * pulse + rnd.uniform(-4, 4)
        tip = 58 - length
        outer = (bezier((60, ey - 11), (42, ey - 12), (tip + 14, ey - 5), (tip, ey))
                 + bezier((tip, ey), (tip + 14, ey + 5), (42, ey + 12), (60, ey + 11)))
        inner_tip = 58 - length * 0.6
        inner = (bezier((60, ey - 6), (48, ey - 6), (inner_tip + 8, ey - 2), (inner_tip, ey))
                 + bezier((inner_tip, ey), (inner_tip + 8, ey + 2), (48, ey + 6), (60, ey + 6)))
        glow(canvas, (52, ey), 30 + 8 * pulse, (255, 140, 40), 0.55)
        flame = Image.new("RGBA", (S, S), (0, 0, 0, 0))
        d = ImageDraw.Draw(flame)
        d.polygon(P(outer), fill=(255, 120 + int(40 * pulse), 30, 235))
        d.polygon(P(inner), fill=(255, 240, 170, 255))
        canvas.alpha_composite(flame.filter(ImageFilter.GaussianBlur(1.2 * SS)))

    # Silhouette outline (drawn once around the union of all hull parts, so it reads well on any background).
    parts = [wing_top, wing_bottom, fin_top, fin_bottom, nacelle_top, nacelle_bottom, fuselage]
    silhouette = mask_of(lambda d: [d.polygon(P(p), fill=255) for p in parts])
    outline(canvas, silhouette, 4, OUTLINE)

    # Wings and fins.
    for p in (wing_top, wing_bottom):
        fill(canvas, mask_of(lambda d: d.polygon(P(p), fill=255)), vertical_gradient(WING_TOP, WING_BOTTOM, 60, 340))
    for p in (wing_stripe_top, wing_stripe_bottom):
        fill(canvas, mask_of(lambda d: d.polygon(P(p), fill=255)), ACCENT)
    for p in (fin_top, fin_bottom):
        fill(canvas, mask_of(lambda d: d.polygon(P(p), fill=255)), ACCENT_DARK)

    # Engine nacelles with dark nozzles and a hot rim.
    for p, ey in zip((nacelle_top, nacelle_bottom), ENGINE_Y):
        fill(canvas, mask_of(lambda d: d.polygon(P(p), fill=255)), vertical_gradient((150, 160, 184, 255), NACELLE, ey - 12, ey + 12))
        fill(canvas, mask_of(lambda d: d.rectangle(P([(54, ey - 12), (64, ey + 12)]), fill=255)), NOZZLE)
        fill(canvas, mask_of(lambda d: d.rectangle(P([(54, ey - 8), (57, ey + 8)]), fill=255)), (255, 170, 80, 255))

    # Fuselage with a lit-from-above gradient, an accent stripe and panel lines.
    fuse_mask = mask_of(lambda d: d.polygon(P(fuselage), fill=255))
    outline(canvas, fuse_mask, 2, OUTLINE)
    fill(canvas, fuse_mask, vertical_gradient(HULL_TOP, HULL_BOTTOM, 155, 245))
    fill(canvas, mask_of(lambda d: d.polygon(P(hull_stripe), fill=255)), ACCENT)
    panel = ImageDraw.Draw(canvas)
    for x in (150, 230):
        panel.line(P([(x, 166), (x, 178)]), fill=(90, 104, 132, 200), width=SS * 2)
        panel.line(P([(x, 222), (x, 234)]), fill=(90, 104, 132, 200), width=SS * 2)

    # Cockpit canopy with a glint that sweeps across it during the loop.
    glass_mask = mask_of(lambda d: d.ellipse(P(cockpit_box), fill=255))
    outline(canvas, glass_mask, 2, OUTLINE)
    fill(canvas, glass_mask, vertical_gradient(GLASS_TOP, GLASS_BOTTOM, 184, 216))
    gx = 262 + (i / (FRAMES - 1)) * 110
    glint = mask_of(lambda d: d.polygon(P([(gx, 182), (gx + 12, 182), (gx - 4, 218), (gx - 16, 218)]), fill=170))
    glint = ImageChops.multiply(glint, glass_mask)
    fill(canvas, glint, (255, 255, 255, 255))
    fill(canvas, mask_of(lambda d: d.ellipse(P([(286, 188), (318, 196)]), fill=200)), (255, 255, 255, 255))

    # Navigation lights: on for the first 3 frames, off for the rest.
    lights_on = i < 3
    for pos, color in NAV_LIGHTS:
        x, y = pos
        if lights_on:
            glow(canvas, pos, 22, color, 0.9)
        c = color if lights_on else tuple(v // 3 for v in color)
        ImageDraw.Draw(canvas).ellipse(P([(x - 5, y - 5), (x + 5, y + 5)]), fill=c + (255,), outline=OUTLINE, width=SS)

    return canvas.resize((SIZE, SIZE), Image.LANCZOS)

if __name__ == "__main__":
    sheet = Image.new("RGBA", (SIZE * FRAMES, SIZE), (0, 0, 0, 0))
    for i in range(FRAMES):
        sheet.paste(draw_frame(i), (i * SIZE, 0))
    sheet.save(sys.argv[1])
    if len(sys.argv) > 2:
        bg = Image.new("RGBA", sheet.size, (14, 16, 34, 255))
        bg.alpha_composite(sheet)
        bg.convert("RGB").save(sys.argv[2])
