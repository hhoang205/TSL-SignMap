import { useState, useEffect } from 'react'

export const useApi = (apiFunction, dependencies = []) => {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    let isMounted = true

    const fetchData = async () => {
      try {
        setLoading(true)
        setError(null)
        const response = await apiFunction()
        if (isMounted) {
          setData(response.data)
        }
      } catch (err) {
        if (isMounted) {
          setError(err.response?.data?.message || err.message || 'An error occurred')
        }
      } finally {
        if (isMounted) {
          setLoading(false)
        }
      }
    }

    fetchData()

    return () => {
      isMounted = false
    }
  }, dependencies)

  return { data, loading, error }
}

