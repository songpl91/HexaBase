using System;
using System.Collections.Generic;
using UnityEngine;
using GridCoordinateSystem.Coordinates;

namespace GridCoordinateSystem.Shapes
{
    /// <summary>
    /// 形状类型枚举
    /// </summary>
    public enum ShapeType
    {
        Cross,      // 十字形
        Diamond,    // 菱形
        Circle,     // 圆形
        Ellipse,    // 椭圆形
        Rectangle,  // 长方形
        Square,     // 正方形
        Sector,     // 扇形
        Line,       // 直线
        Triangle,   // 三角形
        Hexagon,    // 六边形
        Star,       // 星形
        Custom      // 自定义
    }

    /// <summary>
    /// 形状填充模式
    /// </summary>
    public enum FillMode
    {
        Outline,    // 仅边框
        Filled,     // 填充
        Hollow      // 空心（边框+内部空洞）
    }

    /// <summary>
    /// 形状参数配置
    /// </summary>
    [Serializable]
    public class ShapeParameters
    {
        [Header("基础参数")]
        public ShapeType shapeType = ShapeType.Circle;
        public FillMode fillMode = FillMode.Filled;
        public CartesianCoordinate center = CartesianCoordinate.Zero;
        
        [Header("尺寸参数")]
        public int radius = 3;
        public int width = 5;
        public int height = 3;
        public int thickness = 1;
        
        [Header("角度参数（扇形、星形等）")]
        public float startAngle = 0f;
        public float endAngle = 90f;
        public int points = 5; // 星形的角数
        
        [Header("高级参数")]
        public bool includeCenter = true;
        public bool symmetrical = true;
        public float aspectRatio = 1.0f;
        
        /// <summary>
        /// 创建圆形参数
        /// </summary>
        public static ShapeParameters CreateCircle(CartesianCoordinate center, int radius, FillMode fillMode = FillMode.Filled)
        {
            return new ShapeParameters
            {
                shapeType = ShapeType.Circle,
                center = center,
                radius = radius,
                fillMode = fillMode
            };
        }
        
        /// <summary>
        /// 创建矩形参数
        /// </summary>
        public static ShapeParameters CreateRectangle(CartesianCoordinate center, int width, int height, FillMode fillMode = FillMode.Filled)
        {
            return new ShapeParameters
            {
                shapeType = ShapeType.Rectangle,
                center = center,
                width = width,
                height = height,
                fillMode = fillMode
            };
        }
        
        /// <summary>
        /// 创建十字形参数
        /// </summary>
        public static ShapeParameters CreateCross(CartesianCoordinate center, int radius, int thickness = 1)
        {
            return new ShapeParameters
            {
                shapeType = ShapeType.Cross,
                center = center,
                radius = radius,
                thickness = thickness,
                fillMode = FillMode.Filled
            };
        }
        
        /// <summary>
        /// 创建扇形参数
        /// </summary>
        public static ShapeParameters CreateSector(CartesianCoordinate center, int radius, float startAngle, float endAngle, FillMode fillMode = FillMode.Filled)
        {
            return new ShapeParameters
            {
                shapeType = ShapeType.Sector,
                center = center,
                radius = radius,
                startAngle = startAngle,
                endAngle = endAngle,
                fillMode = fillMode
            };
        }
    }

    /// <summary>
    /// 网格形状生成器
    /// 用于在格子系统中生成各种几何形状的坐标集合
    /// </summary>
    public static class GridShapeGenerator
    {
        /// <summary>
        /// 生成指定形状的坐标集合
        /// </summary>
        /// <param name="parameters">形状参数</param>
        /// <returns>坐标集合</returns>
        public static List<CartesianCoordinate> GenerateShape(ShapeParameters parameters)
        {
            switch (parameters.shapeType)
            {
                case ShapeType.Cross:
                    return GenerateCross(parameters);
                case ShapeType.Diamond:
                    return GenerateDiamond(parameters);
                case ShapeType.Circle:
                    return GenerateCircle(parameters);
                case ShapeType.Ellipse:
                    return GenerateEllipse(parameters);
                case ShapeType.Rectangle:
                    return GenerateRectangle(parameters);
                case ShapeType.Square:
                    return GenerateSquare(parameters);
                case ShapeType.Sector:
                    return GenerateSector(parameters);
                case ShapeType.Line:
                    return GenerateLine(parameters);
                case ShapeType.Triangle:
                    return GenerateTriangle(parameters);
                case ShapeType.Hexagon:
                    return GenerateHexagon(parameters);
                case ShapeType.Star:
                    return GenerateStar(parameters);
                default:
                    return new List<CartesianCoordinate>();
            }
        }

        /// <summary>
        /// 生成十字形坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateCross(ShapeParameters parameters)
        {
            var coordinates = new List<CartesianCoordinate>();
            var center = parameters.center;
            int radius = parameters.radius;
            int thickness = Mathf.Max(1, parameters.thickness);
            
            // 水平线
            for (int x = center.X - radius; x <= center.X + radius; x++)
            {
                for (int t = 0; t < thickness; t++)
                {
                    int offsetY = t - thickness / 2;
                    coordinates.Add(new CartesianCoordinate(x, center.Y + offsetY));
                }
            }
            
            // 垂直线
            for (int y = center.Y - radius; y <= center.Y + radius; y++)
            {
                for (int t = 0; t < thickness; t++)
                {
                    int offsetX = t - thickness / 2;
                    var coord = new CartesianCoordinate(center.X + offsetX, y);
                    if (!coordinates.Contains(coord))
                    {
                        coordinates.Add(coord);
                    }
                }
            }
            
            return coordinates;
        }

        /// <summary>
        /// 生成菱形坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateDiamond(ShapeParameters parameters)
        {
            var coordinates = new List<CartesianCoordinate>();
            var center = parameters.center;
            int radius = parameters.radius;
            
            for (int x = center.X - radius; x <= center.X + radius; x++)
            {
                for (int y = center.Y - radius; y <= center.Y + radius; y++)
                {
                    int manhattanDistance = Mathf.Abs(x - center.X) + Mathf.Abs(y - center.Y);
                    
                    bool shouldInclude = false;
                    switch (parameters.fillMode)
                    {
                        case FillMode.Filled:
                            shouldInclude = manhattanDistance <= radius;
                            break;
                        case FillMode.Outline:
                            shouldInclude = manhattanDistance == radius;
                            break;
                        case FillMode.Hollow:
                            shouldInclude = manhattanDistance == radius || 
                                          (manhattanDistance <= radius && manhattanDistance >= radius - parameters.thickness);
                            break;
                    }
                    
                    if (shouldInclude)
                    {
                        coordinates.Add(new CartesianCoordinate(x, y));
                    }
                }
            }
            
            return coordinates;
        }

        /// <summary>
        /// 生成圆形坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateCircle(ShapeParameters parameters)
        {
            var coordinates = new List<CartesianCoordinate>();
            var center = parameters.center;
            int radius = parameters.radius;
            float radiusSquared = radius * radius;
            
            for (int x = center.X - radius; x <= center.X + radius; x++)
            {
                for (int y = center.Y - radius; y <= center.Y + radius; y++)
                {
                    float distanceSquared = (x - center.X) * (x - center.X) + (y - center.Y) * (y - center.Y);
                    
                    bool shouldInclude = false;
                    switch (parameters.fillMode)
                    {
                        case FillMode.Filled:
                            shouldInclude = distanceSquared <= radiusSquared;
                            break;
                        case FillMode.Outline:
                            float distance = Mathf.Sqrt(distanceSquared);
                            shouldInclude = Mathf.Abs(distance - radius) < 0.7f; // 允许一定误差
                            break;
                        case FillMode.Hollow:
                            float dist = Mathf.Sqrt(distanceSquared);
                            float innerRadius = radius - parameters.thickness;
                            shouldInclude = dist <= radius && dist >= innerRadius;
                            break;
                    }
                    
                    if (shouldInclude)
                    {
                        coordinates.Add(new CartesianCoordinate(x, y));
                    }
                }
            }
            
            return coordinates;
        }

        /// <summary>
        /// 生成椭圆形坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateEllipse(ShapeParameters parameters)
        {
            var coordinates = new List<CartesianCoordinate>();
            var center = parameters.center;
            int radiusX = parameters.width / 2;
            int radiusY = parameters.height / 2;
            
            for (int x = center.X - radiusX; x <= center.X + radiusX; x++)
            {
                for (int y = center.Y - radiusY; y <= center.Y + radiusY; y++)
                {
                    float normalizedX = (float)(x - center.X) / radiusX;
                    float normalizedY = (float)(y - center.Y) / radiusY;
                    float ellipseValue = normalizedX * normalizedX + normalizedY * normalizedY;
                    
                    bool shouldInclude = false;
                    switch (parameters.fillMode)
                    {
                        case FillMode.Filled:
                            shouldInclude = ellipseValue <= 1.0f;
                            break;
                        case FillMode.Outline:
                            shouldInclude = Mathf.Abs(ellipseValue - 1.0f) < 0.3f;
                            break;
                        case FillMode.Hollow:
                            float innerFactor = 1.0f - (float)parameters.thickness / Mathf.Min(radiusX, radiusY);
                            shouldInclude = ellipseValue <= 1.0f && ellipseValue >= innerFactor * innerFactor;
                            break;
                    }
                    
                    if (shouldInclude)
                    {
                        coordinates.Add(new CartesianCoordinate(x, y));
                    }
                }
            }
            
            return coordinates;
        }

        /// <summary>
        /// 生成矩形坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateRectangle(ShapeParameters parameters)
        {
            var coordinates = new List<CartesianCoordinate>();
            var center = parameters.center;
            int halfWidth = parameters.width / 2;
            int halfHeight = parameters.height / 2;
            
            int minX = center.X - halfWidth;
            int maxX = center.X + halfWidth;
            int minY = center.Y - halfHeight;
            int maxY = center.Y + halfHeight;
            
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    bool shouldInclude = false;
                    switch (parameters.fillMode)
                    {
                        case FillMode.Filled:
                            shouldInclude = true;
                            break;
                        case FillMode.Outline:
                            shouldInclude = x == minX || x == maxX || y == minY || y == maxY;
                            break;
                        case FillMode.Hollow:
                            bool isOuter = true;
                            bool isInner = x > minX + parameters.thickness - 1 && x < maxX - parameters.thickness + 1 &&
                                          y > minY + parameters.thickness - 1 && y < maxY - parameters.thickness + 1;
                            shouldInclude = isOuter && !isInner;
                            break;
                    }
                    
                    if (shouldInclude)
                    {
                        coordinates.Add(new CartesianCoordinate(x, y));
                    }
                }
            }
            
            return coordinates;
        }

        /// <summary>
        /// 生成正方形坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateSquare(ShapeParameters parameters)
        {
            var squareParams = new ShapeParameters
            {
                shapeType = ShapeType.Rectangle,
                center = parameters.center,
                width = parameters.radius * 2,
                height = parameters.radius * 2,
                fillMode = parameters.fillMode,
                thickness = parameters.thickness
            };
            return GenerateRectangle(squareParams);
        }

        /// <summary>
        /// 生成扇形坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateSector(ShapeParameters parameters)
        {
            var coordinates = new List<CartesianCoordinate>();
            var center = parameters.center;
            int radius = parameters.radius;
            float startAngle = parameters.startAngle * Mathf.Deg2Rad;
            float endAngle = parameters.endAngle * Mathf.Deg2Rad;
            
            // 确保角度范围正确
            if (endAngle < startAngle)
            {
                endAngle += 2 * Mathf.PI;
            }
            
            for (int x = center.X - radius; x <= center.X + radius; x++)
            {
                for (int y = center.Y - radius; y <= center.Y + radius; y++)
                {
                    float distance = Mathf.Sqrt((x - center.X) * (x - center.X) + (y - center.Y) * (y - center.Y));
                    
                    if (distance <= radius)
                    {
                        float angle = Mathf.Atan2(y - center.Y, x - center.X);
                        if (angle < 0) angle += 2 * Mathf.PI;
                        
                        // 检查角度是否在扇形范围内
                        bool inAngleRange = false;
                        if (endAngle - startAngle >= 2 * Mathf.PI)
                        {
                            inAngleRange = true; // 完整圆形
                        }
                        else if (endAngle <= 2 * Mathf.PI)
                        {
                            inAngleRange = angle >= startAngle && angle <= endAngle;
                        }
                        else
                        {
                            inAngleRange = angle >= startAngle || angle <= (endAngle - 2 * Mathf.PI);
                        }
                        
                        bool shouldInclude = false;
                        switch (parameters.fillMode)
                        {
                            case FillMode.Filled:
                                shouldInclude = inAngleRange;
                                break;
                            case FillMode.Outline:
                                bool isEdge = Mathf.Abs(distance - radius) < 0.7f ||
                                             (inAngleRange && (Mathf.Abs(angle - startAngle) < 0.1f || Mathf.Abs(angle - endAngle) < 0.1f));
                                shouldInclude = inAngleRange && isEdge;
                                break;
                            case FillMode.Hollow:
                                float innerRadius = radius - parameters.thickness;
                                shouldInclude = inAngleRange && distance >= innerRadius;
                                break;
                        }
                        
                        if (shouldInclude)
                        {
                            coordinates.Add(new CartesianCoordinate(x, y));
                        }
                    }
                }
            }
            
            return coordinates;
        }

        /// <summary>
        /// 生成直线坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateLine(ShapeParameters parameters)
        {
            var coordinates = new List<CartesianCoordinate>();
            var center = parameters.center;
            int length = parameters.radius;
            float angle = parameters.startAngle * Mathf.Deg2Rad;
            
            for (int i = -length; i <= length; i++)
            {
                int x = center.X + Mathf.RoundToInt(i * Mathf.Cos(angle));
                int y = center.Y + Mathf.RoundToInt(i * Mathf.Sin(angle));
                coordinates.Add(new CartesianCoordinate(x, y));
            }
            
            return coordinates;
        }

        /// <summary>
        /// 生成三角形坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateTriangle(ShapeParameters parameters)
        {
            var coordinates = new List<CartesianCoordinate>();
            var center = parameters.center;
            int radius = parameters.radius;
            
            // 等边三角形的三个顶点
            var vertices = new List<CartesianCoordinate>
            {
                new CartesianCoordinate(center.X, center.Y + radius),
                new CartesianCoordinate(center.X - radius, center.Y - radius/2),
                new CartesianCoordinate(center.X + radius, center.Y - radius/2)
            };
            
            for (int x = center.X - radius; x <= center.X + radius; x++)
            {
                for (int y = center.Y - radius; y <= center.Y + radius; y++)
                {
                    bool isInside = IsPointInTriangle(new CartesianCoordinate(x, y), vertices[0], vertices[1], vertices[2]);
                    
                    if (isInside && parameters.fillMode == FillMode.Filled)
                    {
                        coordinates.Add(new CartesianCoordinate(x, y));
                    }
                    else if (parameters.fillMode == FillMode.Outline)
                    {
                        // 检查是否在三角形边上
                        if (IsPointOnTriangleEdge(new CartesianCoordinate(x, y), vertices[0], vertices[1], vertices[2]))
                        {
                            coordinates.Add(new CartesianCoordinate(x, y));
                        }
                    }
                }
            }
            
            return coordinates;
        }

        /// <summary>
        /// 生成六边形坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateHexagon(ShapeParameters parameters)
        {
            var coordinates = new List<CartesianCoordinate>();
            var center = parameters.center;
            int radius = parameters.radius;
            
            for (int x = center.X - radius; x <= center.X + radius; x++)
            {
                for (int y = center.Y - radius; y <= center.Y + radius; y++)
                {
                    if (IsPointInHexagon(new CartesianCoordinate(x, y), center, radius))
                    {
                        coordinates.Add(new CartesianCoordinate(x, y));
                    }
                }
            }
            
            return coordinates;
        }

        /// <summary>
        /// 生成星形坐标
        /// </summary>
        private static List<CartesianCoordinate> GenerateStar(ShapeParameters parameters)
        {
            var coordinates = new List<CartesianCoordinate>();
            var center = parameters.center;
            int outerRadius = parameters.radius;
            int innerRadius = outerRadius / 2;
            int points = parameters.points;
            
            for (int x = center.X - outerRadius; x <= center.X + outerRadius; x++)
            {
                for (int y = center.Y - outerRadius; y <= center.Y + outerRadius; y++)
                {
                    if (IsPointInStar(new CartesianCoordinate(x, y), center, outerRadius, innerRadius, points))
                    {
                        coordinates.Add(new CartesianCoordinate(x, y));
                    }
                }
            }
            
            return coordinates;
        }

        #region 辅助方法

        /// <summary>
        /// 判断点是否在三角形内
        /// </summary>
        private static bool IsPointInTriangle(CartesianCoordinate p, CartesianCoordinate a, CartesianCoordinate b, CartesianCoordinate c)
        {
            float denom = (b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y);
            if (Mathf.Abs(denom) < 0.001f) return false;
            
            float alpha = ((b.Y - c.Y) * (p.X - c.X) + (c.X - b.X) * (p.Y - c.Y)) / denom;
            float beta = ((c.Y - a.Y) * (p.X - c.X) + (a.X - c.X) * (p.Y - c.Y)) / denom;
            float gamma = 1 - alpha - beta;
            
            return alpha >= 0 && beta >= 0 && gamma >= 0;
        }

        /// <summary>
        /// 判断点是否在三角形边上
        /// </summary>
        private static bool IsPointOnTriangleEdge(CartesianCoordinate p, CartesianCoordinate a, CartesianCoordinate b, CartesianCoordinate c)
        {
            return IsPointOnLineSegment(p, a, b) || IsPointOnLineSegment(p, b, c) || IsPointOnLineSegment(p, c, a);
        }

        /// <summary>
        /// 判断点是否在线段上
        /// </summary>
        private static bool IsPointOnLineSegment(CartesianCoordinate p, CartesianCoordinate a, CartesianCoordinate b)
        {
            float distance = Mathf.Abs((b.Y - a.Y) * p.X - (b.X - a.X) * p.Y + b.X * a.Y - b.Y * a.X) /
                           Mathf.Sqrt((b.Y - a.Y) * (b.Y - a.Y) + (b.X - a.X) * (b.X - a.X));
            
            return distance < 0.7f && 
                   p.X >= Mathf.Min(a.X, b.X) && p.X <= Mathf.Max(a.X, b.X) &&
                   p.Y >= Mathf.Min(a.Y, b.Y) && p.Y <= Mathf.Max(a.Y, b.Y);
        }

        /// <summary>
        /// 判断点是否在六边形内
        /// </summary>
        private static bool IsPointInHexagon(CartesianCoordinate point, CartesianCoordinate center, int radius)
        {
            float dx = point.X - center.X;
            float dy = point.Y - center.Y;
            
            // 使用六边形的数学定义
            float distance = Mathf.Max(
                Mathf.Abs(dx),
                Mathf.Max(
                    Mathf.Abs(dx * 0.5f + dy * 0.866f),
                    Mathf.Abs(dx * 0.5f - dy * 0.866f)
                )
            );
            
            return distance <= radius;
        }

        /// <summary>
        /// 判断点是否在星形内
        /// </summary>
        private static bool IsPointInStar(CartesianCoordinate point, CartesianCoordinate center, int outerRadius, int innerRadius, int points)
        {
            float dx = point.X - center.X;
            float dy = point.Y - center.Y;
            float distance = Mathf.Sqrt(dx * dx + dy * dy);
            float angle = Mathf.Atan2(dy, dx);
            if (angle < 0) angle += 2 * Mathf.PI;
            
            float anglePerPoint = 2 * Mathf.PI / points;
            float normalizedAngle = (angle % anglePerPoint) / anglePerPoint;
            
            // 在星形的尖角和凹角之间插值
            float targetRadius;
            if (normalizedAngle < 0.5f)
            {
                targetRadius = Mathf.Lerp(outerRadius, innerRadius, normalizedAngle * 2);
            }
            else
            {
                targetRadius = Mathf.Lerp(innerRadius, outerRadius, (normalizedAngle - 0.5f) * 2);
            }
            
            return distance <= targetRadius;
        }

        #endregion
    }
}