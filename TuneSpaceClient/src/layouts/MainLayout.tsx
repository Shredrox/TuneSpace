import { Outlet } from 'react-router-dom'
import Header from '../components/Header'
import Sidebar from '../components/Sidebar'

const MainLayout = () => {
  return (
    <>
      <Header/>
      <div className='min-h-screen'>
        <Sidebar/>
        <main className='ml-[150px] mt-16 p-6'>
          <Outlet />
        </main>
      </div>
    </>
  )
}

export default MainLayout
