import { RouterProvider } from "react-router-dom";
import { MainRouter } from "./router/MainRouter";
import { ThemeProvider } from "./components/ThemeProvider";

function App() {
  return (
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <RouterProvider router={MainRouter}/>
    </ThemeProvider>
  )
}

export default App
