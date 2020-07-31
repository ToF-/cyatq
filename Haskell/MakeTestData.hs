import System.Random
import System.Environment


queries x y = [(i,j) | i <- [x..y], j <- [i..y]]
segment ls x y = take (y-x+1) $ drop (x-1) ls
segmentSum ls x y = sum $ segment ls x y 
maximumSum :: [Int] -> Int -> Int -> Int
maximumSum ls x y = maximum $ map (\(i,j) -> segmentSum ls i j) $ queries x y
main = do
  g <- getStdGen
  n <- getArgs

  let maxNumber = read (n!!0)
  let numbers = take maxNumber $ map (`rem` 150) (randoms g :: [Int])
  putStrLn $ "< " ++ (show maxNumber)
  putStr "<"
  mapM (\n -> putStr (" " ++(show n))) numbers
  putStrLn ""
  putStrLn ("< " ++ (show (length (queries 1 maxNumber))))
  mapM (\(x,y) -> do
        putStrLn ("< " ++ (show x) ++ " " ++ (show y))
        putStrLn ("> " ++ (show  $ (maximumSum numbers x y))))
        (queries 1 maxNumber)
