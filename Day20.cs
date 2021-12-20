class Day20 : IDayCommand {

    public bool TransformToBinary(char input) => input == '#';

    public bool GetRealValueConsideringInfinity(Point2D coords, Dictionary<Point2D, bool> image,  int enhancementStep, List<bool> decoder) {
        // It is considered an infinity point, if neither it exists, or its neighbors exist
        if(image.ContainsKey(coords)) return image[coords];

        // if the decoder at 000000000 is true, it negates the input each step, so in an odd step it is true and in an even step it is false
        if(decoder[0] && enhancementStep % 2 == 1) return true;
        return false;
    }   
    
    public Dictionary<Point2D, bool> GenerateImage(List<string> lines) {
        var image = new Dictionary<Point2D, bool>();
        for (int i = 0; i < lines.Count(); i++)
        {
            for (int j = 0; j < lines[i].Count(); j++)
            {
                    image.Add(new (i,j), TransformToBinary(lines[i][j]));
            }
        }
        return image;
    }

    public bool GeneratePixel(Point2D coord, Dictionary<Point2D, bool> inputImage, int enhancementStep, List<bool> decoder) {
        var code = coord.GenerateAdjacent(includeDiagonal: true, includePoint: true)
            .Select(p => GetRealValueConsideringInfinity(p, inputImage, enhancementStep, decoder))
            .Select(p => p? "1" : "0");
        var codeString = string.Concat(code);
        var index = Convert.ToInt32(codeString, 2);
        return decoder[index];
    }

    public Dictionary<Point2D, bool> EnhanceImage(Dictionary<Point2D, bool> inputImage, int enhancementStep, List<bool> decoder) {        
        var outputImage = new Dictionary<Point2D, bool>();
        inputImage.Keys.ToList().ForEach(pixel => {
            var all = pixel.GenerateAdjacent(includeDiagonal:true);

            all.ForEach(p => {
                if(!inputImage.ContainsKey(p) && !outputImage.ContainsKey(p)) {
                    outputImage.Add(p, GeneratePixel(p, inputImage, enhancementStep, decoder));
                }
            });

            // Process current pixel
            outputImage.Add(pixel, GeneratePixel(pixel, inputImage, enhancementStep, decoder));
        });
        return outputImage;
    }

    public void PrintImage(Dictionary<Point2D, bool> inputImage) {
        var maxX = inputImage.Keys.Max(k => k.x);
        var maxY = inputImage.Keys.Max(k => k.y);
        var minX = inputImage.Keys.Min(k => k.x);
        var minY = inputImage.Keys.Min(k => k.y);

        for (var i = minX; i <= maxX; i++)
        {
            for (var j = minY; j <= maxY; j++)
            {
                if(!inputImage.ContainsKey(new(i,j))) {
                    Console.Write(".");
                    continue;
                }
                Console.Write(inputImage[new(i,j)]? "#" : ".");
            }
            Console.Write(Environment.NewLine);
        }
    }

    public string Execute() {

        var input = new FileReader(20).Read().ToList();

        var decoder = input.First().Select(c => TransformToBinary(c)).ToList();
        var image = GenerateImage(input.Skip(2).ToList());
        var litPixelPart01 = 0;

        for (int i = 0; i < 50; i++)
        {
            image = EnhanceImage(image, i, decoder);
            if(i == 1) litPixelPart01 = image.Values.Sum(v => v? 1 : 0);
        }

        var litPixelPart02 = image.Values.Sum(v => v? 1 : 0);

        //PrintImage(image);
        
        return $"The number of lit pixels after 2 enhancement is {litPixelPart01} and after 50 enhancements is {litPixelPart02}";
    }

}